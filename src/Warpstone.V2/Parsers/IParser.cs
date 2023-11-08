using Warpstone.V2.Alt;
using Warpstone.V2.Errors;

namespace Warpstone.V2.Parsers;

public interface IParser
{
    public Type ResultType { get; }

    public void Step(IActiveParseContext context, int position, int phase);

    public IParseResult Fail(int position, IParseInput input);

    public IParseResult Mismatch(int position, IEnumerable<IParseError> errors);

    public IParseResult Match(int position, int length, object value);

    public IterativeStep Eval(IParseInput input, int position, Func<IParser, int, IterativeStep> eval);

    public string ToString(int depth);
}

public interface IParser<T> : IParser
{
    public new IParseResult<T> Fail(int position, IParseInput input);

    public new IParseResult<T> Mismatch(int position, IEnumerable<IParseError> errors);

    public IParseResult<T> Match(int position, int length, T value);
}
