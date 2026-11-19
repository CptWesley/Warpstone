namespace Warpstone.Tests.Legacy;

/// <summary>
/// Tests ported from the V1 <c>CommonParsers</c> test class.
/// Only the whitespace tests are ported; V3 has no Alphanumeric, Letter, UppercaseLetter,
/// LowercaseLetter, Digit, Newline, single Whitespace or Trim parsers (yet).
/// </summary>
public static class CommonParsersTests
{
    public static readonly IEnumerable<object[]> Options = BasicParsersTests.Options;

    /// <summary>
    /// Checks that parsing whitespaces works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(WhitespacesInputs))]
    public static void WhitespacesCorrect(string input, ParseOptions options)
        => AssertThat(Whitespaces.Parse(input, options)).IsEqualTo(input);

    /// <summary>
    /// Checks that parsing whitespaces works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void WhitespacesParserIncorrect(ParseOptions options)
        => AssertThat(() => Whitespaces.Parse(string.Empty, options)).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that parsing optional whitespaces works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(OptionalWhitespacesInputs))]
    public static void OptionalWhitespacesCorrect(string input, ParseOptions options)
        => AssertThat(OptionalWhitespaces.Parse(input, options)).IsEqualTo(input);

    public static IEnumerable<object[]> WhitespacesInputs()
        => Combine("\n", "\r\n", " ", "\t", "\r\n\n\t ");

    public static IEnumerable<object[]> OptionalWhitespacesInputs()
        => Combine(string.Empty, "\n", "\r\n", " ", "\t", "\r\n\n\t ");

    private static IEnumerable<object[]> Combine(params string[] inputs)
        => inputs.SelectMany(input => Options.Select(o => new object[] { input, o[0] }));
}
