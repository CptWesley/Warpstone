using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Warpstone.Examples.Json
{
    public abstract class JsonValue : IParsed
    {
        public ParseInputPosition Position { get; set; }
    }

    public abstract class JsonValue<T> : JsonValue
    {
        public JsonValue(T value)
            => Value = value;

        public T Value { get; }

        public override string ToString()
            => $"{GetType().Name}({Value})";
    }

    public class JsonInt : JsonValue<int>
    {
        public JsonInt(int value) : base(value)
        {
        }
    }

    public class JsonString : JsonValue<string>
    {
        public JsonString(string value) : base(value)
        {
        }

        public override string ToString()
            => $"JsonString(\"{Value}\")";
    }

    public class JsonArray : JsonValue<ImmutableArray<JsonValue>>
    {
        public JsonArray(ImmutableArray<JsonValue> value) : base(value)
        {
        }

        public override string ToString()
            => $"JsonArray([{string.Join(", ", Value)}])";
    }

    public class JsonDouble : JsonValue<double>
    {
        public JsonDouble(double value) : base(value)
        {
        }
    }

    public class JsonObject : JsonValue<ImmutableArray<KeyValuePair<JsonString, JsonValue>>>
    {
        public JsonObject(ImmutableArray<KeyValuePair<JsonString, JsonValue>> value) : base(value)
        {
        }

        public override string ToString()
            => $"JsonObject({string.Join(", ", Value.Select(x => $"{x.Key}: {x.Value}"))})";
    }

    public class JsonBoolean : JsonValue<bool>
    {
        public JsonBoolean(bool value) : base(value)
        {
        }
    }

    public class JsonNull : JsonValue
    {
        public override string ToString()
            => "JsonNull()";
    }
}
