using System;

namespace Warpstone.Examples.Json
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            ParseResult<JsonValue> result = JsonParser.Parse(@"{""elements"": [null, null], ""name"": ""NullContainer"" }");
            Console.WriteLine(result.Value);
        }
    }
}
