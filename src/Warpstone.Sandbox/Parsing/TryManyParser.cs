using Warpstone;

namespace ClausewitzLsp.Core.Parsing;

/// <summary>
/// Parser which tries to parse many elements.
/// </summary>
/// <typeparam name="TElement">Type of the elements.</typeparam>
internal class TryManyParser<TElement> : Parser<IList<ParseOption<TElement>>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TryManyParser{TElement}"/> class.
    /// </summary>
    /// <param name="prefix">The prefix parser.</param>
    /// <param name="element">The element parser.</param>
    /// <param name="delimiter">The delimiter parser.</param>
    /// <param name="suffix">The suffix parser.</param>
    /// <param name="recoveryParser">The parser used to reach a recovery point in case of failure.</param>
    public TryManyParser(IParser<string> prefix, IParser<TElement> element, IParser<string> delimiter, IParser<string> suffix, IParser<string> recoveryParser)
            => (Prefix, Element, Delimiter, Suffix, RecoveryParser) = (prefix, element, delimiter, suffix, recoveryParser);

    /// <summary>
    /// Gets the prefix parser.
    /// </summary>
    public IParser<string> Prefix { get; }

    /// <summary>
    /// Gets the element parser.
    /// </summary>
    public IParser<TElement> Element { get; }

    /// <summary>
    /// Gets the delimiter parser.
    /// </summary>
    public IParser<string> Delimiter { get; }

    /// <summary>
    /// Gets the suffix parser.
    /// </summary>
    public IParser<string> Suffix { get; }

    /// <summary>
    /// Gets the recovery parser.
    /// </summary>
    public IParser<string> RecoveryParser { get; }

    /// <inheritdoc/>
    public override IParseResult<IList<ParseOption<TElement>>> TryParse(string input, int position)
    {
        List<IParseResult> results = new List<IParseResult>();
        IParseResult<string> prefix = Prefix.TryParse(input, position);
        results.Add(prefix);
        int endPosition = prefix.Position.End;

        if (!prefix.Success)
        {
            return new ParseResult<IList<ParseOption<TElement>>>(this, input, position, endPosition, prefix.Error!, results);
        }

        List<ParseOption<TElement>> values = new List<ParseOption<TElement>>();

        IParseResult<string> directSuffix = Suffix.TryParse(input, endPosition);
        if (directSuffix.Success)
        {
            endPosition = directSuffix.Position.End;
            results.Add(directSuffix);
            return new ParseResult<IList<ParseOption<TElement>>>(this, values, input, position, endPosition, results);
        }

        IParseResult<TElement> first = Element.TryParse(input, endPosition);
        endPosition = first.Position.End;
        results.Add(first);
        if (first.Success)
        {
            values.Add(new ParseSome<TElement>(first.Value!));
        }
        else
        {
            values.Add(new ParseNone<TElement>(first.Error!));
            IParseResult<string> firstRecovery = RecoveryParser.TryParse(input, endPosition);
            endPosition = firstRecovery.Position.End;
        }

        while (true)
        {
            IParseResult<string> suffix = Suffix.TryParse(input, endPosition);
            if (suffix.Success)
            {
                endPosition = suffix.Position.End;
                results.Add(suffix);
                return new ParseResult<IList<ParseOption<TElement>>>(this, values, input, position, endPosition, results);
            }

            if (endPosition >= input.Length)
            {
                HashSet<string> expected = new HashSet<string>();
                expected.UnionWith(GetExpected(Delimiter));
                expected.UnionWith(GetExpected(Suffix));

                return new ParseResult<IList<ParseOption<TElement>>>(this, input, position, endPosition, new UnexpectedTokenError(new SourcePosition(input, endPosition, endPosition), expected, GetFound(input, endPosition)), results);
            }

            IParseResult<string> delimiter = Delimiter.TryParse(input, endPosition);
            endPosition = delimiter.Position.End;
            results.Add(delimiter);
            if (!delimiter.Success)
            {
                values.Add(new ParseNone<TElement>(delimiter.Error!));
                IParseResult<string> delimiterRecovery = RecoveryParser.TryParse(input, endPosition);
                endPosition = delimiterRecovery.Position.End;
                continue;
            }

            IParseResult<TElement> element = Element.TryParse(input, endPosition);
            endPosition = element.Position.End;
            results.Add(element);
            if (first.Success)
            {
                values.Add(new ParseSome<TElement>(element.Value!));
            }
            else
            {
                values.Add(new ParseNone<TElement>(element.Error!));
                IParseResult<string> elementRecovery = RecoveryParser.TryParse(input, endPosition);
                endPosition = elementRecovery.Position.End;
            }
        }
    }

    /// <inheritdoc/>
    public override string ToString(int depth)
    {
        if (depth < 0)
        {
            return "...";
        }

        return $"TryMany({Prefix.ToString(depth - 1)}, {Element.ToString(depth - 1)}, {Delimiter.ToString(depth - 1)}, {Suffix.ToString(depth - 1)})";
    }

    private IEnumerable<string> GetExpected(IParser parser)
    {
        IParseResult result = Element.TryParse(string.Empty, 0);
        if (!result.Success && result.Error is UnexpectedTokenError error)
        {
            return error.Expected;
        }

        return Array.Empty<string>();
    }
}