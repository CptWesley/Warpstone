using System;

namespace Warpstone.Examples.Json
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Parse(@"
{
    ""elements"": [null, null],
    ""name"": ""NullContainer"",
    ""body"": ""This is the first paragraph\nThis is the second paragraph"",
    ""inner"": {
        ""numVal"": 42,
        ""boolVal"": true
    }
}");
        }

        public static void Parse(string input)
        {
            var res = JsonParser.Parse(input);
            Console.WriteLine(res.Position);
            Console.WriteLine(res);
        }
    }
}
