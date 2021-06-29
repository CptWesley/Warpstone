using System.Collections.Generic;
using System.Collections.Immutable;
using static Warpstone.Parsers.BasicParsers;
using static Warpstone.Parsers.CommonParsers;

namespace Warpstone.Examples.Json
{
    public static class JsonParser
    {
        public static readonly IParser<JsonNull> Null
            = String("null").Then(Create(new JsonNull()));

        public static readonly IParser<JsonInt> Int
            = OneOrMore(Digit).Concat().Transform(x => new JsonInt(int.Parse(x)));

        public static readonly IParser<JsonDouble> Double
            = Regex(@"[0-9]*\.[0-9]+")
            .Transform(x => new JsonDouble(double.Parse(x)));

        public static readonly IParser<JsonArray> Array
            = Char('[').Then(Many(Lazy(() => Json), Char(','))).ThenSkip(Char(']'))
            .Transform(x => new JsonArray(x.ToImmutableArray()));

        public static readonly IParser<JsonBoolean> True
            = String("true").Transform(x => new JsonBoolean(true));

        public static readonly IParser<JsonBoolean> False
            = String("false").Transform(x => new JsonBoolean(false));

        public static readonly IParser<JsonBoolean> Boolean
            = Or(True, False);

        public static readonly IParser<string> StringContent
            = Or(Regex("[^\"\\n\\r]"), String("\\\""));

        public static readonly IParser<JsonString> String
            = Char('"').Then(Many(StringContent).Concat()).ThenSkip(Char('"'))
            .Transform(x => new JsonString(System.Text.RegularExpressions.Regex.Unescape(x))).Highlight(Highlight.String);

        public static readonly IParser<KeyValuePair<JsonString, JsonValue>> Field
            = OptionalWhitespaces
            .Then(String)
            .ThenSkip(OptionalWhitespaces)
            .ThenSkip(Char(':'))
            .ThenSkip(OptionalWhitespaces)
            .ThenAdd(Lazy(() => Json))
            .Transform((x, y) => new KeyValuePair<JsonString, JsonValue>(x, y));

        public static readonly IParser<JsonObject> Object
            = Char('{').Then(Many(Field, Char(','), Char('}')))
            .Transform(x => new JsonObject(x.ToImmutableArray()));

        public static readonly IParser<JsonValue> Constant
            = Or<JsonValue>(Null, Double, Int, Boolean).Highlight(Highlight.Constant);

        public static readonly IParser<JsonValue> Json
            = OptionalWhitespaces.Then(Or(Array, Object, String, Constant)).ThenSkip(OptionalWhitespaces);

        public static JsonValue Parse(string input)
            => Json.ThenEnd().Parse(input);

        public static string GetGrammar()
            => Json.ToTextMateGrammar("mylang", "MyLang");

        private static IParser<string> Concat<T>(this IParser<IEnumerable<T>> chars)
            => chars.Transform(x => string.Join(string.Empty, x));
    }
}
