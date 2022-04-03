using ClausewitzLsp.Core.Parsing;
using System.Diagnostics;

namespace Warpstone.Sandbox;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");

        string input = @"
{
    a
    b
    nob
    c
    foo = [
    blA
}

xat = {
 a b c
}

cax = {
 { date = 40 foo = [ }
 { }
}

z = ---
";
        //input = inputFile;
        var result = Parser.FileBody.TryParse(input);
        var errors = result.Value.GetErrors();
        var messages = errors.Select(x => x.GetMessage());
        var joined = string.Join("\n", messages);
        Console.WriteLine(joined);

        var foo = Parser.Identifier.TryParse("ara_sindicat_remença_flag");
        Console.WriteLine(foo);

        List<ParseFileExpr> files = new List<ParseFileExpr>();

        Stopwatch sw = new Stopwatch();
        sw.Start();

        foreach (string file in Directory.GetFiles(@"C:\Program Files (x86)\Steam\steamapps\common\Europa Universalis IV\events", "*.txt", SearchOption.AllDirectories))
        {
            ParseFileExpr parsedFile = Parser.ParseFile(file);
            Console.WriteLine($"=== {parsedFile.FileName}");
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (IParseError error in parsedFile.Errors)
            {
                Console.WriteLine(error.GetMessage());
            }

            files.Add(parsedFile);


            Console.ResetColor();
        }

        sw.Stop();
        Console.WriteLine("Elapsed={0}", sw.Elapsed);

        Console.WriteLine("done");
    }
}