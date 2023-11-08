using Warpstone.V2.Errors;

namespace Warpstone.V2.Parsers;

public abstract class ParserBase<T> : IParser<T>
{
    public Type ResultType => typeof(T);

    public IParseResult<T> Fail(int position, IParseInput input)
        => ParseResult.CreateFail(this, position, input);

    IParseResult IParser.Fail(int position, IParseInput input)
        => Fail(position, input);

    public abstract IterativeStep Eval(IParseInput input, int position, Func<IParser, int, IterativeStep> eval);

    public IParseResult<T> Mismatch(int position, IEnumerable<IParseError> errors)
        => ParseResult.CreateMismatch(this, position, errors);

    public IParseResult<T> Match(int position, int length, T value)
        => ParseResult.CreateMatch(this, position, length, value);

    IParseResult IParser.Mismatch(int position, IEnumerable<IParseError> errors)
        => Mismatch(position, errors);

    IParseResult IParser.Match(int position, int length, object value)
    {
        if (value is not T v)
        {
            throw new ArgumentException($"Argument is not of type '{typeof(T).FullName}'.", nameof(value));
        }

        return Match(position, length, v);
    }

    protected abstract string InternalToString(int depth);

    public string ToString(int depth)
    {
        if (depth < 0)
        {
            return "...";
        }

        return InternalToString(depth);
    }

    public sealed override string ToString()
        => ToString(10);
}
