using static Warpstone.Parsers;

namespace Warpstone.Sandbox;

public static class Program
{
    private static readonly IParser<string> Parser
        = Or(
            String("a").ThenAdd(Lazy(() => Parser!)).Transform(p => p.Left + p.Right),
            End);

    public static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Input:");

            var input = Console.ReadLine() ?? string.Empty;

            if (input == "exit")
            {
                break;
            }

            var result = Parser.TryParse(input, ParseOptions.Default with { ExecutionMode = ParserExecutionMode.Iterative });

            if (result.Success)
            {
                Console.WriteLine(result.Value);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error);
                }
            }

            Console.WriteLine();
        }
    }
}
