using Legacy.Warpstone2.Errors;
using Legacy.Warpstone2.Internal;
using Legacy.Warpstone2.IterativeExecution;
using System.Collections.Immutable;

namespace Legacy.Warpstone2.Parsers.Internal;

/// <summary>
/// Parser which succeeds if the inner parser succeeds, but does not consume any tokens.
/// </summary>
/// <typeparam name="T">The type of the wrapped parser.</typeparam>
internal sealed class PositiveLookaheadParser<T> : ParserBase<T>, IParserFirst<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PositiveLookaheadParser{T}"/> class.
    /// </summary>
    /// <param name="first">The inner parser that is being wrapped.</param>
    public PositiveLookaheadParser(IParser<T> first)
    {
        First = first;
    }

    /// <summary>
    /// The inner parser.
    /// </summary>
    public IParser<T> First { get; }

    /// <inheritdoc />
    public override IIterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IIterativeStep> eval)
        => Iterative.More(
            () => eval(First, position),
            untypedFirst =>
            {
                var first = untypedFirst.AssertOfType<IParseResult<T>>();

                if (first.Success)
                {
                    return Iterative.Done(this.Match(context, position, 0, first.Value));
                }
                else
                {
                    return Iterative.Done(this.Mismatch(context, position, first.Errors));
                }
            });

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => $"Peek({First.ToString(depth - 1)})";
}
