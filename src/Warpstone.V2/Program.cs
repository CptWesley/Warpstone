namespace Warpstone.V2;

using System.Runtime.CompilerServices;

public static class Program
{
    public static void Main(string[] args)
    {
        var parser = new SequenceParser<string, (string, string)>(
            new CharParser('a'),
            new SequenceParser<string, string>(
                new CharParser('b'),
                new ChoiceParser<string>(
                    new CharParser('c'),
                    new CharParser('d'))));
        var result1 = parser.Parse("abc");
        var result2 = parser.Parse("abd");
        var result3 = parser.Parse("abe");
    }
}
