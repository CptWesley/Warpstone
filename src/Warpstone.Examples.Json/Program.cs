using System;

namespace Warpstone.Examples.Json
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Parse(@"{""elements"": [null, null], ""name"": ""NullContainer"", ""body"": ""This is the first paragraph\nThis is the second paragraph"" }");
        }

        public static void Parse(string input)
            => Console.WriteLine(JsonParser.Parse(input));
    }
}
