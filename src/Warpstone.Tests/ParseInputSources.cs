using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Warpstone.Tests;

public sealed class ParseInputSources
{
    // Left recursion, backtracking between "two" and "three", whitespace and end of input: every parser that reads input except Regex.
    private static readonly IParser<int> sum = CreateSum();

    private static readonly string longSum = string.Join(" + ", Enumerable.Repeat("one + two +\nthree", 1000));

    private static IParser<int> CreateSum()
    {
        var num = Or(String("one").Transform(_ => 1), String("three").Transform(_ => 3), String("two").Transform(_ => 2));
        IParser<int> expr = null!;
        expr = Or(
            Lazy(() => expr).ThenSkip(OptionalWhitespaces).ThenSkip(Char('+')).ThenSkip(Whitespaces).ThenAdd(num).Transform((l, r) => l + r),
            num);
        return expr.ThenSkip(OptionalWhitespaces).ThenEnd();
    }

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public void Reader_parses_like_string(ParseOptions options)
    {
        sum.Parse(ParseInput.CreateFromReader(new StringReader(longSum)), options).Should().Be(6000);
    }

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public void Reader_fails_like_string(ParseOptions options)
    {
        var input = longSum + " + four";
        var expected = sum.TryParse(input, options);
        var actual = sum.TryParse(ParseInput.CreateFromReader(new StringReader(input)), options);

        actual.Success.Should().BeFalse();
        actual.Errors.Select(Describe).Should().Equal(expected.Errors.Select(Describe));
    }

    private static (int Position, string? Message) Describe(IParseError error)
        => (error.Position, (error as Exception)?.Message);

    [Test]
    public void Reader_is_read_lazily()
    {
        using var reader = new StringReader("one" + new string('x', 100_000));

        String("one").Parse(ParseInput.CreateFromReader(reader)).Should().Be("one");
        reader.ReadToEnd().Length.Should().BeGreaterThan(90_000);
    }

    [Test]
    public void Content_reads_the_whole_input()
    {
        var text = "one" + new string('x', 100_000);
        var input = ParseInput.CreateFromReader(new StringReader(text));

        String("one").Parse(input);
        input.Content.Should().Be(text);
    }

    [Test]
    public void TryGetChar_and_Substring_read_up_to_the_index()
    {
        using var reader = new StringReader("abc" + new string('x', 100_000));
        var input = ParseInput.CreateFromReader(reader);

        input.TryGetChar(1, out var c).Should().BeTrue();
        c.Should().Be('b');
        input.Substring(0, 3).Should().Be("abc");
        reader.ReadToEnd().Length.Should().BeGreaterThan(90_000);

        input.TryGetChar(100_003, out _).Should().BeFalse();
        input.Invoking(i => i.Substring(100_000, 4)).Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public void Reader_matches_culture_aware_strings_like_string(ParseOptions options)
    {
        var parser = String("ONE", ignoreCase: true).ThenSkip(OptionalWhitespaces).ThenAdd(String("Two", ignoreCase: true)).ThenEnd();

        parser.Parse(ParseInput.CreateFromReader(new StringReader("one  TWO")), options)
            .Should().Be(parser.Parse("one  TWO", options));
    }

    [Test]
    public void Regex_reads_the_whole_input()
    {
        var text = "one" + new string('x', 100_000);
        var input = ParseInput.CreateFromReader(new StringReader(text));

        Regex("o").Parse(input).Should().Be("o");
        input.Content.Should().Be(text);
    }

    [Test]
    public void Reader_positions_match_string_positions()
    {
        var text = "ab\n\ncd\ne\n";
        var expected = ParseInput.CreateFromMemory(text);
        var actual = ParseInput.CreateFromReader(new StringReader(text));
        Regex(".*", System.Text.RegularExpressions.RegexOptions.Singleline).Parse(actual);

        for (var i = 0; i <= text.Length; i++)
        {
            (actual.GetPosition(i).Line, actual.GetPosition(i).Column).Should().Be((expected.GetPosition(i).Line, expected.GetPosition(i).Column), $"index {i}");
        }
    }

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public async Task Async_parse_reads_input_as_it_arrives(ParseOptions options)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var channel = Channel.CreateUnbounded<string>();
        var reader = new ChannelTextReader(channel.Reader);

        var parse = sum.ParseAsync(ParseInput.CreateFromReader(reader), options, timeout.Token);
        await channel.Writer.WriteAsync(longSum[..5000], timeout.Token);

        // The parser takes the whole first chunk, then waits for more.
        while (reader.Delivered < 5000)
        {
            await Task.Delay(1, timeout.Token);
        }

        parse.IsCompleted.Should().BeFalse();

        await channel.Writer.WriteAsync(longSum[5000..], timeout.Token);
        channel.Writer.Complete();

        (await parse).Should().Be(6000);
    }

    [Test]
    public async Task Async_parse_of_string_input()
    {
        (await sum.ParseAsync(ParseInput.CreateFromMemory(longSum))).Should().Be(6000);
    }

    [Test]
    public async Task Async_parse_can_be_cancelled()
    {
        using var cancellation = new CancellationTokenSource();
        var channel = Channel.CreateUnbounded<string>();
        var parse = sum.TryParseAsync(ParseInput.CreateFromReader(new ChannelTextReader(channel.Reader)), cancellation.Token);

        cancellation.Cancel();
        channel.Writer.TryWrite("one");

        await parse.Awaiting(p => p).Should().ThrowAsync<OperationCanceledException>();
    }

    [Test]
    public void Reads_stream_strips_bom_and_leaves_it_open()
    {
        using var stream = new MemoryStream([.. Encoding.UTF8.GetPreamble(), .. Encoding.UTF8.GetBytes("one + two")]);
        var input = ParseInput.CreateFromStream(stream);

        sum.Parse(input).Should().Be(3);
        input.Source.Should().Be(FromMemorySource.Instance);
        stream.CanRead.Should().BeTrue();
    }

    [Test]
    public void File_stream_gets_file_source()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "one + two");
            using var stream = File.OpenRead(path);
            var input = ParseInput.CreateFromStream(stream);

            sum.Parse(input).Should().Be(3);
            input.Source.Should().BeOfType<FileSource>().Which.Path.Should().Be(stream.Name);
        }
        finally
        {
            File.Delete(path);
        }
    }

    /// <summary>Reads chunks from a channel, and only asynchronously, so a blocking read fails the test.</summary>
    private sealed class ChannelTextReader(ChannelReader<string> chunks) : TextReader
    {
        private string chunk = string.Empty;
        private int offset;

        public int Delivered { get; private set; }

        public override int Read(char[] buffer, int index, int count)
            => throw new InvalidOperationException("Read synchronously.");

        public override async Task<int> ReadAsync(char[] buffer, int index, int count)
        {
            while (offset == chunk.Length)
            {
                if (!await chunks.WaitToReadAsync())
                {
                    return 0;
                }

                chunks.TryRead(out var next);
                chunk = next ?? string.Empty;
                offset = 0;
            }

            var read = Math.Min(count, chunk.Length - offset);
            chunk.CopyTo(offset, buffer, index, read);
            offset += read;
            Delivered += read;
            return read;
        }
    }
}
