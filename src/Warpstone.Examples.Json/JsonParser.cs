using System.Collections.Generic;
using System.Collections.Immutable;
using static Warpstone.Parsers.BasicParsers;
using static Warpstone.Parsers.CommonParsers;
using static Warpstone.Parsers.ExpressionParsers;

namespace Warpstone.Examples.Json
{
    public static class JsonParser
    {
        private static readonly IParser<JsonValue> Null
            = String("null").Then(Create(new JsonNull() as JsonValue));

        private static readonly IParser<JsonValue> Int
            = OneOrMore(Digit).Concat().Transform(x => new JsonInt(int.Parse(x)) as JsonValue);

        private static readonly IParser<JsonValue> Double
            = Regex(@"[0-9]*\.[0-9]+")
            .Transform(x => new JsonDouble(double.Parse(x)) as JsonValue);

        private static readonly IParser<JsonValue> Array
            = Char('[').Then(Many(Lazy(() => Json), Char(','))).ThenSkip(Char(']'))
            .Transform(x => new JsonArray(x.ToImmutableArray()) as JsonValue);

        private static readonly IParser<JsonValue> True
            = String("true").Transform(x => new JsonBoolean(true) as JsonValue);

        private static readonly IParser<JsonValue> False
            = String("false").Transform(x => new JsonBoolean(false) as JsonValue);

        private static readonly IParser<JsonValue> Boolean
            = Or(True, False);

        private static readonly IParser<string> StringContent
            = Or(Regex("[^\"\\n\\r]"), String("\\\""));

        private static readonly IParser<JsonValue> String
            = Char('"').Then(Many(StringContent).Concat()).ThenSkip(Char('"'))
            .Transform(x => new JsonString(System.Text.RegularExpressions.Regex.Unescape(x)) as JsonValue);

        private static readonly IParser<KeyValuePair<JsonString, JsonValue>> Field
            = OptionalWhitespaces
            .Then(String.Transform(x => x as JsonString))
            .ThenSkip(OptionalWhitespaces)
            .ThenSkip(Char(':'))
            .ThenSkip(OptionalWhitespaces)
            .ThenAdd(Lazy(() => Json))
            .Transform((x, y) => new KeyValuePair<JsonString, JsonValue>(x, y));

        private static readonly IParser<JsonValue> Object
            = Char('{').Then(Many(Field, Char(','))).ThenSkip(Char('}'))
            .Transform(x => new JsonObject(x.ToImmutableArray()) as JsonValue);

        private static readonly IParser<JsonValue> Json
            = OptionalWhitespaces.Then(Or(Array, Object, String, Null, Double, Int, Boolean)).ThenSkip(OptionalWhitespaces);

        public static JsonValue Parse(string input)
            => Json.ThenEnd().Parse(input);

        private static IParser<string> Concat<T>(this IParser<IEnumerable<T>> chars)
            => chars.Transform(x => string.Join(string.Empty, x));
    }
}
