using Legacy.Warpstone2.Errors;
using Legacy.Warpstone2.Internal;
using Legacy.Warpstone2.IterativeExecution;
using System.Collections.Immutable;

namespace Legacy.Warpstone2.Parsers.Internal;

/// <summary>
/// Parser which grants a name to the expected input.
/// </summary>
/// <typeparam name="T">The type of the wrapped parser.</typeparam>
internal sealed class ExpectedParser<T> : ParserBase<T>, IParserValue<IImmutableList<string>>, IParserFirst<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExpectedParser{T}"/> class.
    /// </summary>
    /// <param name="first">The inner parser that is being wrapped.</param>
    /// <param name="value">The expected element name.</param>
    public ExpectedParser(IParser<T> first, IEnumerable<string> value)
    {
        First = first;
        Expected = value.ToImmutableArray();
    }

    /// <summary>
    /// The inner parser.
    /// </summary>
    public IParser<T> First { get; }

    /// <summary>
    /// The expected element name.
    /// </summary>
    public IImmutableList<string> Expected { get; }

    /// <inheritdoc />
    IImmutableList<string> IParserValue<IImmutableList<string>>.Value => Expected;

    /// <inheritdoc />
    public override IIterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IIterativeStep> eval)
        => Iterative.More(
            () => eval(First, position),
            untypedFirst =>
            {
                var first = untypedFirst.AssertOfType<IParseResult<T>>();

                if (first.Success)
                {
                    return Iterative.Done(this.Match(context, position, first.Length, first.Value));
                }
                else
                {
                    return Iterative.Done(this.Mismatch(
                        context,
                        position,
                        new UnexpectedTokenError(context, this, position, 1, Expected, Array.Empty<UnexpectedTokenError>())));
                }
            });

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => string.Join(" | ", Expected);
}
