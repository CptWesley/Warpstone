using System.Text;
using Warpstone.Parsers;

namespace Warpstone.Sandbox;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");

        IParser<string> num = new RegexParser(@"0|-?[1-9][0-9]*", true);

        IParser<(string, string)> all = new SeqParser<string, string>(num, new EndParser());

        IParser<string> rec = null!;
        rec = new LazyParser<string>(() => rec);

        //Console.WriteLine(all.Parse("42x", 0, 2));
        //Console.WriteLine(rec.Parse(""));
        GenerateClass();
    }

    private static void GenerateClass()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("namespace Warpstone.Parsers;");
        sb.AppendLine();
        
        for (int count = 2; count <= 16; count++)
        {
            IEnumerable<int> range = Enumerable.Range(1, count);
            string tupleString = $"({string.Join(", ", range.Select(x => $"T{x} Value{x}"))})";
            string shortString = string.Join(", ", range.Select(x => $"T{x}"));
            sb.AppendLine("/// <summary>")
                .AppendLine("/// Represents a parser which sequentially applies a sequence of parsers and retains all results.")
                .AppendLine("/// </summary>");

            foreach (int i in range)
            {
                sb.AppendLine($"/// <typeparam name=\"T{i}\">The result type of the {ToOrdinal(i)} parser.</typeparam>");
            }

            sb.AppendLine("/// <seealso cref=\"Parser{T}\" />")
                .Append("public sealed class SeqParser<")
                .Append(shortString)
                .AppendLine(">")
                .AppendLine($"\t: BaseSeqParser<{tupleString}>")
                .AppendLine("{")
                .AppendLine("\t/// <summary>")
                .AppendLine($"\t/// Initializes a new instance of the <see cref=\"SeqParser{{{shortString}}}\"/> class.")
                .AppendLine("\t/// </summary>");

            foreach (int i in range)
            {
                sb.AppendLine($"\t/// <param name=\"{ToOrdinal(i)}\">The result type of the {ToOrdinal(i)} parser.</param>");
            }

            string constructorArgs = string.Join(", ", range.Select(x => $"IParser<T{x}> {ToOrdinal(x)}"));

            sb.AppendLine($"\tpublic SeqParser({constructorArgs})")
                .AppendLine($"\t\t: base({string.Join(", ", range.Select(ToOrdinal))})")
                .AppendLine("\t{")
                .AppendLine("\t}")
                .AppendLine();

            sb.AppendLine("\t/// <inheritdoc/>")
                .AppendLine("\t[MethodImpl(MethodImplOptions.AggressiveInlining)]")
                .AppendLine($"\tprotected sealed override {tupleString} CreateValue(object?[] values)")
                .AppendLine($"\t\t=> ({string.Join(", ", range.Select(x => $"(T{x})values[{x - 1}]!"))});");

            sb.AppendLine("}")
                .AppendLine();
        }


        Console.WriteLine(sb);
    }

    public static string ToOrdinal(int num)
        => num switch
        {
            1 => "first",
            2 => "second",
            3 => "third",
            4 => "fourth",
            5 => "fifth",
            6 => "sixth",
            7 => "seventh",
            8 => "eighth",
            9 => "nineth",
            10 => "tenth",
            11 => "eleventh",
            12 => "twelfth",
            13 => "thirteenth",
            14 => "fourteenth",
            15 => "fifteenth",
            16 => "sixteenth",
            _ => throw new Exception()
        };
}