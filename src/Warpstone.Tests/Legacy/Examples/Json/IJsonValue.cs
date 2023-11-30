using System.Collections.Immutable;

namespace Warpstone.Tests.Legacy.Examples.Json;

public interface IJsonValue
{
}

public abstract class JsonValue<T> : IJsonValue
{
    protected JsonValue(T value)
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

public class JsonArray : JsonValue<ImmutableArray<IJsonValue>>
{
    public JsonArray(ImmutableArray<IJsonValue> value) : base(value)
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

public class JsonObject : JsonValue<ImmutableArray<KeyValuePair<JsonString, IJsonValue>>>
{
    public JsonObject(ImmutableArray<KeyValuePair<JsonString, IJsonValue>> value) : base(value)
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

public class JsonNull : IJsonValue
{
    public override string ToString()
        => "JsonNull()";
}
