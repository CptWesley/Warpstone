namespace Warpstone.Sandbox;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");

        var input = ParseInput.CreateFromMemory(@"
From Wikipedia, the free encyclopedia
Packrat parserClass	Parsing grammars that are PEG
Data structure	String
Worst-case performance	O ( n ) O(n) or O ( n 2 ) O(n^{2}) without special handling of iterative combinator
Best-case performance	

    O ( n ) O(n)

Average performance	O ( n ) O(n)
Worst-case space complexity	O ( n ) O(n)

The Packrat parser is a type of parser that shares similarities with the recursive descent parser in its construction. However, it differs because it takes parsing expression grammars (PEGs) as input rather than LL grammars.[1]

In 1970, A. Birman laid the groundwork for packrat parsing by introducing the TMG recognition schema (TS). His work was later refined by Aho and Ullman and renamed as Generalized Top-Down Parsing Language (GTDPL). This algorithm was the first of its kind to employ deterministic top-down parsing with backtracking.[2][3]

Bryan Ford developed PEGs as an expansion of GTDPL and TS. Unlike CFGs, PEGs are unambiguous and can match well with machine-oriented languages. PEGs, similar to GTDPL and TS, can also express all LL(k) and LR(k). Bryan also introduced Packrat as a parser that uses memoization techniques on top of a simple PEG parser. This was done because PEGs have an unlimited lookahead capability resulting in a parser with exponential time performance in the worst case.[2][3]

Packrat keeps track of the intermediate results for all mutually recursive parsing functions. Each parsing function is only called once at a specific input position. In some instances of packrat implementation, if there is insufficient memory, certain parsing functions may need to be called multiple times at the same input position, causing the parser to take longer than linear time.[4]
");

        Console.WriteLine($"Length: {input.Content.Length}");
        for (var i = 0; i < input.Content.Length; i++)
        {
            Console.WriteLine($"{i} [{Regex.Escape(input.Content[i].ToString())}]: {input.GetPosition(i)}");
        }
    }
}
