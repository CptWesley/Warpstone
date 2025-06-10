namespace Warpstone.Internal.ParserImplementations;

/// <summary>
/// Represents a parser that parses either the provided first or second option.
/// </summary>
/// <typeparam name="T">The result type of the parsers.</typeparam>
internal sealed class OrParserImpl<T> : ParserImplementationBase<OrParser<T>, T>
{
    private IParserImplementation<T> first = default!;
    private IParserImplementation<T> second = default!;
    private Continuation continuation = default!;

    /// <inheritdoc />
    protected override void InitializeInternal(OrParser<T> parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
    {
        first = (IParserImplementation<T>)parserLookup[parser.First];
        second = (IParserImplementation<T>)parserLookup[parser.Second];
        continuation = new(this, second);
    }

    /// <inheritdoc />
    public override void Apply(IIterativeParseContext context, int position)
    {
        context.ExecutionStack.Push((position, continuation));
        context.ExecutionStack.Push((position, first));
    }

    /// <inheritdoc />
    public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var left = first.Apply(context, position);

        if (left.Success)
        {
            return left;
        }

        var right = second.Apply(context, left.NextPosition);

        if (right.Success)
        {
            return right;
        }

        return new(position, JoinErrors(context, this, left.Errors, right.Errors));
    }

    /// <summary>
    /// The continuation parser when executing in iterative mode.
    /// </summary>
    /// <param name="Root">The root parser.</param>
    /// <param name="Second">The second parser to try.</param>
    private sealed class Continuation(IParserImplementation Root, IParserImplementation Second) : ContinuationParserImplementationBase
    {
        /// <summary>
        /// The second continuation of the sequential parser when executing in iterative mode.
        /// </summary>
        public SecondContinuation Continue { get; } = new(Root);

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            var leftResult = context.ResultStack.Peek();

            if (leftResult.Success)
            {
                return;
            }

            context.ExecutionStack.Push((position, Continue));
            context.ExecutionStack.Push((position, Second));
        }
    }

    /// <summary>
    /// The second continuation of the choice parser when executing in iterative mode.
    /// </summary>
    private sealed class SecondContinuation(IParserImplementation Root) : ContinuationParserImplementationBase
    {
        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            var right = context.ResultStack.Pop();
            var left = context.ResultStack.Pop();

            if (right.Success)
            {
                context.ResultStack.Push(right);
                return;
            }

            context.ResultStack.Push(new(left.Position, JoinErrors(context, Root, left.Errors, right.Errors)));
        }
    }

    private static IEnumerable<IParseError> JoinErrors(IParseContext context, IParserImplementation parser, IEnumerable<IParseError>? left, IEnumerable<IParseError>? right)
    {
        left ??= [];
        right ??= [];

        var joined = left.Concat(right);
        var dict = new Dictionary<int, (HashSet<string> Set, HashSet<UnexpectedTokenError> Inner)>();
        var other = new List<IParseError>();

        foreach (var error in joined)
        {
            if (error is not UnexpectedTokenError te)
            {
                other.Add(error);
            }
            else
            {
                if (!dict.TryGetValue(te.Position, out var pair))
                {
                    pair = (new(), new());
                    dict.Add(te.Position, pair);
                }

                pair.Inner.Add(te);

                foreach (var e in te.Expected)
                {
                    pair.Set.Add(e);
                }
            }
        }

        foreach (var error in other)
        {
            yield return error;
        }

        foreach (var entry in dict)
        {
            yield return new UnexpectedTokenError(
                context: context,
                parser: parser,
                position: entry.Key,
                length: 1,
                expected: entry.Value.Set,
                innerErrors: entry.Value.Inner);
        }
    }
}
