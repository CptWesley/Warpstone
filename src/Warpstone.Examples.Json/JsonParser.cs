using System.Collections.Generic;
using System.Collections.Immutable;
using static Warpstone.Parsers;

namespace Warpstone.Examples.Json
{
    public static class JsonParser
    {
        private static readonly Parser<string> OptionalWhitespaces
            = Many(Whitespace).Transform(x => string.Join(string.Empty, x));

        private static readonly Parser<JsonValue> Null
            = String("null").Then(Create(new JsonNull() as JsonValue));

        private static readonly Parser<JsonValue> Int
            = OneOrMore(Digit).Concat().Transform(x => new JsonInt(int.Parse(x)) as JsonValue);

        private static readonly Parser<JsonValue> Double
            = Many(Digit).Concat().ThenSkip(Char('.')).ThenAdd(OneOrMore(Digit).Concat())
            .Transform(x => new JsonDouble(double.Parse($"{x.Item1}.{x.Item2}")) as JsonValue);

        private static readonly Parser<JsonValue> Array
            = Char('[').Then(Many(Lazy(() => Json), Char(','))).ThenSkip(Char(']'))
            .Transform(x => new JsonArray(x.ToImmutableArray()) as JsonValue);

        private static readonly Parser<JsonValue> True
            = String("true").Transform(x => new JsonBoolean(true) as JsonValue);

        private static readonly Parser<JsonValue> False
            = String("false").Transform(x => new JsonBoolean(false) as JsonValue);

        private static readonly Parser<JsonValue> Boolean
            = Or(True, False);

        private static readonly Parser<string> StringContent
            = Or(
                String("\\n").Transform(x => "\n"),
                String("\\r").Transform(x => "\t"),
                String("\\\"").Transform(x => "\""),
                String("\\\\").Transform(x => "\\"),
                Alphanumeric.Transform(x => x.ToString()),
                Char(' ').Transform(x => x.ToString()));

        private static readonly Parser<JsonValue> String
            = Char('"').Then(Many(StringContent).Concat()).ThenSkip(Char('"'))
            .Transform(x => new JsonString(x) as JsonValue);

        private static readonly Parser<KeyValuePair<JsonString, JsonValue>> Field
            = OptionalWhitespaces
            .Then(String.Transform(x => x as JsonString))
            .ThenSkip(OptionalWhitespaces)
            .ThenSkip(Char(':'))
            .ThenSkip(OptionalWhitespaces)
            .ThenAdd(Lazy(() => Json))
            .Transform(x => new KeyValuePair<JsonString, JsonValue>(x.Item1, x.Item2));

        private static readonly Parser<JsonValue> Object
            = Char('{').Then(Many(Field, Char(','))).ThenSkip(Char('}'))
            .Transform(x => new JsonObject(x.ToImmutableArray()) as JsonValue);

        private static readonly Parser<JsonValue> Json
            = OptionalWhitespaces.Then(Or(Array, Object, String, Null, Double, Int, Boolean)).ThenSkip(OptionalWhitespaces);

        public static ParseResult<JsonValue> Parse(string input)
            => Json.ThenEnd().Parse(input);

        private static Parser<string> Concat<T>(this Parser<IEnumerable<T>> chars)
            => chars.Transform(x => string.Join(string.Empty, x));
    }
}
