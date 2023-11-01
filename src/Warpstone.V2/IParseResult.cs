using Warpstone.V2.Errors;
using Warpstone.V2.Parsers;

namespace Warpstone.V2;

public interface IParseResult
{
    public int Position { get; }

    public int Length { get; }

    public int NextPosition { get; }

    public IParser Parser { get; }

    public ParseStatus Status { get; }

    public object? Value { get; }

    public IReadOnlyList<IParseError> Errors { get; }

    public IParseResult AsMismatch();
}


public interface IParseResult<T> : IParseResult
{
    public new IParser<T> Parser { get; }

    public new T Value { get; }

    public new IParseResult<T> AsMismatch();
}
