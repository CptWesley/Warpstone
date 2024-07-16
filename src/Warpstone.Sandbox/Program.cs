using Warpstone.Examples.Legacy.Json;

namespace Warpstone.Sandbox;

public static class Program
{
    public static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Input:");

            var input = Console.ReadLine();

            if (input == "exit")
            {
                break;
            }

            var result = JsonParser.Parse(input!);

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
