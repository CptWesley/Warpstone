namespace Warpstone.Parsers.Internal;

/// <summary>
/// Parser which attempts to parse the input in two different ways.
/// If the first parser fails, the second parser is used.
/// If the second parser also fails, this parser will fail.
/// </summary>
/// <typeparam name="T">The result type of the parsers.</typeparam>
internal sealed class ChoiceParser<T> : ParserBase<T>, IParserSecond<T, T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChoiceParser{T}"/> class.
    /// </summary>
    /// <param name="first">The parser which is attempted first.</param>
    /// <param name="second">The parser which is attempted second.</param>
    public ChoiceParser(IParser<T> first, IParser<T> second)
    {
        First = first;
        Second = second;
    }

    /// <summary>
    /// The parser which is attempted first.
    /// </summary>
    public IParser<T> First { get; }

    /// <summary>
    /// The parser which is attempted second.
    /// </summary>
    public IParser<T> Second { get; }

    /// <inheritdoc />
    public override IIterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IIterativeStep> eval)
        => Iterative.More(
            () => eval(First, position),
            untypedFirst =>
            {
                var first = untypedFirst.AssertOfType<IParseResult<T>>();

                if (first.Success)
                {
                    return Iterative.Done(first);
                }

                return Iterative.More(
                    () => eval(Second, position),
                    untypedSecond =>
                    {
                        var second = untypedSecond.AssertOfType<IParseResult<T>>();

                        if (second.Success)
                        {
                            return Iterative.Done(second);
                        }

                        var errors = MergeErrors(first.Errors, second.Errors);
                        return Iterative.Done(this.Mismatch(context, position, errors));
                    });
            });

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => $"({First.ToString(depth - 1)} | {Second.ToString(depth - 1)})";

    private IEnumerable<IParseError> MergeErrors(IReadOnlyList<IParseError> first, IReadOnlyList<IParseError> second)
    {
        var errors = new List<IParseError>();
        var tokenErrors = new Dictionary<int, UnexpectedTokenError>();

        foreach (var error in first.Concat(second))
        {
            if (error is UnexpectedTokenError ute)
            {
                if (tokenErrors.TryGetValue(ute.Position, out var prev))
                {
                    tokenErrors[ute.Position] = Merge(prev, ute);
                }
                else
                {
                    tokenErrors.Add(ute.Position, ute);
                }
            }
            else
            {
                errors.Add(error);
            }
        }

        errors.AddRange(tokenErrors.Values);
        return errors;
    }

    private UnexpectedTokenError Merge(UnexpectedTokenError first, UnexpectedTokenError second)
    {
        var expected = first.Expected.Concat(second.Expected);
        var length = first.Length == second.Length ? first.Length : 1;
        return new UnexpectedTokenError(first.Context, this, first.Position, length, expected, new[] { first, second });
    }
}
