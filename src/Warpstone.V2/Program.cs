namespace Warpstone.V2;

using System.Runtime.CompilerServices;
using Warpstone.V2.Parsers;

public static class Program
{
    public static void Main(string[] args)
    {
        var parser = new SequenceParser<char, (char, char)>(
            new CharParser('a'),
            new SequenceParser<char, char>(
                new CharParser('b'),
                new ChoiceParser<char>(
                    new CharParser('c'),
                    new CharParser('d'))));
        var result1 = parser.Parse("abc");
        var result2 = parser.Parse("abd");
        var result3 = parser.Parse("abe");
    }
}
