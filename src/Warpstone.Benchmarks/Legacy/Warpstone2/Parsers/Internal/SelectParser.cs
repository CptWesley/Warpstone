using Legacy.Warpstone2.Errors;
using Legacy.Warpstone2.Internal;
using Legacy.Warpstone2.IterativeExecution;
using System.Collections.Immutable;

namespace Legacy.Warpstone2.Parsers.Internal;

/// <summary>
/// A parser which first attempts to use the <see cref="First"/> parser
/// and then if it succeeds uses the <see cref="Select"/> function to
/// determine which parser is the next one to be executed.
/// </summary>
/// <typeparam name="TFirst">The result type of the first parser.</typeparam>
/// <typeparam name="TSecond">The result type of the parser selected by this parser.</typeparam>
internal sealed class SelectParser<TFirst, TSecond> : ParserBase<(TFirst, TSecond)>, IParserFirst<TFirst>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectParser{TIn, TOut}"/> class.
    /// </summary>
    /// <param name="first">The parser to attempt first.</param>
    /// <param name="select">The function which determines the next parser to be used.</param>
    public SelectParser(IParser<TFirst> first, Func<TFirst, IParser<TSecond>> select)
    {
        First = first;
        Select = select;
    }

    /// <inheritdoc />
    public IParser<TFirst> First { get; }

    /// <summary>
    /// The function which determines the next parser to be used.
    /// </summary>
    public Func<TFirst, IParser<TSecond>> Select { get; }

    /// <inheritdoc />
    public override IIterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IIterativeStep> eval)
        => Iterative.More(
            () => eval(First, position),
            untypedInner =>
            {
                var first = untypedInner.AssertOfType<IParseResult<TFirst>>();

                if (!first.Success)
                {
                    return Iterative.Done(this.Mismatch(context, position, first.Errors));
                }

                IParser<TSecond> second;
                try
                {
                    second = Select(first.Value);
                }
                catch (Exception e)
                {
                    var error = new TransformationError(context, this, position, 0, e.Message, e);
                    return Iterative.Done(this.Mismatch(context, position, error));
                }

                return Iterative.More(
                    () => eval(second, first.NextPosition),
                    untypedSecond =>
                    {
                        var second = untypedSecond.AssertOfType<IParseResult<TSecond>>();

                        if (!second.Success)
                        {
                            return Iterative.Done(this.Mismatch(context, position, second.Errors));
                        }

                        var value = (first.Value, second.Value);
                        var length = first.Length + second.Length;
                        return Iterative.Done(this.Match(context, position, length, value));
                    });
            });

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => $"Select({First.ToString(depth - 1)})";
}
