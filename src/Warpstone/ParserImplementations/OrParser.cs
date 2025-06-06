namespace Warpstone.ParserImplementations;

/// <summary>
/// Represents a parser that parses either the provided first or second option.
/// </summary>
/// <typeparam name="T">The result type of the parsers.</typeparam>
internal sealed class OrParser<T> : IParser<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrParser{T}"/> class.
    /// </summary>
    /// <param name="first">The first parser to try.</param>
    /// <param name="second">The second parser to try.</param>
    public OrParser(IParser<T> first, IParser<T> second)
    {
        First = first;
        Second = second;
        Continue = new(this, second);
    }

    /// <summary>
    /// The first parser to try.
    /// </summary>
    public IParser<T> First { get; }

    /// <summary>
    /// The second parser to try.
    /// </summary>
    public IParser<T> Second { get; }

    /// <summary>
    /// The continuation parser when executing in iterative mode.
    /// </summary>
    private Continuation Continue { get; }

    /// <inheritdoc />
    public Type ResultType => typeof(T);

    /// <inheritdoc />
    public void Apply(IIterativeParseContext context, int position)
    {
        context.ExecutionStack.Push((position, Continue));
        context.ExecutionStack.Push((position, First));
    }

    /// <inheritdoc />
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var left = First.Apply(context, position);

        if (left.Success)
        {
            return left;
        }

        var right = Second.Apply(context, left.NextPosition);

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
    private sealed class Continuation(IParser Root, IParser Second) : IParser
    {
        /// <summary>
        /// The second continuation of the sequential parser when executing in iterative mode.
        /// </summary>
        public SecondContinuation Continue { get; } = new(Root);

        /// <inheritdoc />
        public Type ResultType => throw new NotSupportedException();

        /// <inheritdoc />
        public void Apply(IIterativeParseContext context, int position)
        {
            var leftResult = context.ResultStack.Peek();

            if (leftResult.Success)
            {
                return;
            }

            context.ExecutionStack.Push((position, Continue));
            context.ExecutionStack.Push((position, Second));
        }

        /// <inheritdoc />
        public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
            => throw new NotSupportedException();
    }

    /// <summary>
    /// The second continuation of the choice parser when executing in iterative mode.
    /// </summary>
    private sealed class SecondContinuation(IParser Root) : IParser
    {
        /// <inheritdoc />
        public Type ResultType => throw new NotSupportedException();

        /// <inheritdoc />
        public void Apply(IIterativeParseContext context, int position)
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

        /// <inheritdoc />
        public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
            => throw new NotSupportedException();
    }

    private static IEnumerable<IParseError> JoinErrors(IParseContext context, IParser parser, IEnumerable<IParseError>? left, IEnumerable<IParseError>? right)
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
