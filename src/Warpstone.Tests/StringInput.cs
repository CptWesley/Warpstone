namespace Warpstone.Tests;

public static class StringInput
{
    [TestCase(4, 2, 2)]
    [TestCase(7, 3, 1)]
    [TestCase(9, 3, 3)]
    public static void Finds_line_and_column(int index, int line, int column)
    {
        var position = ParseInput.CreateFromMemory("ab\ncd\r\nef").GetPosition(index);

        position.Line.Should().Be(line);
        position.Column.Should().Be(column);
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("\n    ")]
    [TestCase("\r\n  ")]
    [TestCase("\t ")]
    [TestCase("\n\n")]
    public static void Returns_the_skipped_whitespace(string whitespace)
        => OptionalWhitespaces.ThenSkip(Char('x')).Parse(whitespace + "x").Should().Be(whitespace);
}
