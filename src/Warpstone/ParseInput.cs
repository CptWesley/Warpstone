namespace Warpstone;

/// <summary>
/// Represents information for inputs used by parsers.
/// Basic concrete implementation of the <see cref="IParseInput"/> interface.
/// </summary>
public sealed class ParseInput : IParseInput
{
    private readonly int[] lineLengths;

    private ParseInput(IParseInputSource source, string content, int[] lineLengths)
    {
        Source = source;
        Content = content;
        this.lineLengths = lineLengths;
    }

    /// <inheritdoc />
    public string Content { get; }

    /// <inheritdoc />
    public IParseInputSource Source { get; }

    /// <inheritdoc />
    public LineColumn GetPosition(int index)
    {
        var start = 0;
        var end = lineLengths.Length;
        var range = end;

        while (range > 1)
        {
            var halfRange = range / 2;
            var pivot = start + halfRange;

            var startIndex = lineLengths[pivot];

            if (index < startIndex)
            {
                end -= halfRange;
                range = end - start;
            }
            else if (index == startIndex)
            {
                return new LineColumn(index, pivot + 1, 1);
            }
            else if (index > startIndex)
            {
                if (pivot == lineLengths.Length - 1)
                {
                    return new LineColumn(index, pivot + 1, index - startIndex + 1);
                }

                var nextStartIndex = lineLengths[pivot + 1];
                if (index < nextStartIndex)
                {
                    return new LineColumn(index, pivot + 1, index - startIndex + 1);
                }

                start += halfRange;
                range = end - start;
            }
        }

        return new LineColumn(index, start + 1, index - lineLengths[start] + 1);
    }

    /// <summary>
    /// Creates a new <see cref="IParseInput"/> instance
    /// from the given <paramref name="content"/> string
    /// with with the given <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The source of the input.</param>
    /// <param name="content">The input string.</param>
    /// <returns>A new <see cref="IParseInput"/> instance.</returns>
    public static IParseInput Create(IParseInputSource source, string content)
    {
        var lines = new List<int>
        {
            0,
        };

        var acc = 0;
        foreach (var line in content.Split('\n'))
        {
            acc += line.Length + 1;
            lines.Add(acc);
        }

        return new ParseInput(source, content, lines.ToArray());
    }

    /// <summary>
    /// Creates a new <see cref="IParseInput"/> instance
    /// from the given <paramref name="input"/> string
    /// with a from-memory source.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>A new <see cref="IParseInput"/> instance.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseInput CreateFromMemory(string input)
        => Create(FromMemorySource.Instance, input);

    /// <summary>
    /// Creates a new <see cref="IParseInput"/> instance
    /// from the given <paramref name="content"/> string
    /// with a file source from the given <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path to the source file.</param>
    /// <param name="content">The input string.</param>
    /// <returns>A new <see cref="IParseInput"/> instance.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseInput CreateFromFile(string path, string content)
        => Create(new FileSource(path), content);

    /// <summary>
    /// Creates a new <see cref="IParseInput"/> instance
    /// from the given <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path to the source file.</param>
    /// <returns>A new <see cref="IParseInput"/> instance.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseInput CreateFromFile(string path)
        => CreateFromFile(path, File.ReadAllText(path));
}
