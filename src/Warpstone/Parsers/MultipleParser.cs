using System.Collections.Generic;

namespace Warpstone.Parsers;

/// <summary>
/// A parser which runs a single parser multiple times with optional
/// delimiter and terminator parsers.
/// </summary>
/// <typeparam name="TElement">The parser for the elements.</typeparam>
/// <typeparam name="TDelimiter">The parser for the delimiter.</typeparam>
/// <typeparam name="TTerminator">The parser for the terminator.</typeparam>
public sealed class MultipleParser<TElement, TDelimiter, TTerminator> :
    ParserBase<IImmutableList<TElement>>,
    IParserThird<TElement, TDelimiter, TTerminator>,
    IParserValue<(ulong Min, ulong Max)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleParser{TElement, TDelimiter, TTerminator}"/> class.
    /// </summary>
    /// <param name="element">The parser for the elements.</param>
    /// <param name="delimiter">The parser for the delimiter.</param>
    /// <param name="terminator">The parser for the terminator.</param>
    /// <param name="min">The minimum number of matched elements.</param>
    /// <param name="max">The maximum number of matched elements.</param>
    public MultipleParser(
        IParser<TElement> element,
        IParser<TDelimiter> delimiter,
        IParser<TTerminator> terminator,
        ulong min,
        ulong max)
    {
        Element = element;
        Delimiter = delimiter;
        Terminator = terminator;
        Min = min;
        Max = max;
    }

    /// <summary>
    /// The parser for the elements.
    /// </summary>
    public IParser<TElement> Element { get; }

    /// <summary>
    /// The parser for the delimiter.
    /// </summary>
    public IParser<TDelimiter> Delimiter { get; }

    /// <summary>
    /// The parser for the terminator.
    /// </summary>
    public IParser<TTerminator> Terminator { get; }

    /// <summary>
    /// The minimum number of matched elements.
    /// </summary>
    public ulong Min { get; }

    /// <summary>
    /// The maximum number of matched elements.
    /// </summary>
    public ulong Max { get; }

    /// <inheritdoc />
    IParser<TElement> IParserFirst<TElement>.First => Element;

    /// <inheritdoc />
    IParser<TDelimiter> IParserSecond<TDelimiter>.Second => Delimiter;

    /// <inheritdoc />
    IParser<TTerminator> IParserThird<TTerminator>.Third => Terminator;

    /// <inheritdoc />
    (ulong Min, ulong Max) IParserValue<(ulong Min, ulong Max)>.Value => (Min, Max);

    /// <inheritdoc />
    public override IterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IterativeStep> eval)
        => Iterative.Done(() => eval(Element, position));
        /*
        => Iterative.More(
            () => eval(First, position),
            untypedFirst =>
            {
                var first = untypedFirst.AssertOfType<IParseResult<TElement>>();

                if (!first.Success)
                {
                    return Iterative.Done(this.Mismatch(context, position, first.Errors));
                }

                return Iterative.More(
                    () => eval(Second, first.NextPosition),
                    untypedSecond =>
                    {
                        var second = untypedSecond.AssertOfType<IParseResult<TSecond>>();

                        if (!second.Success)
                        {
                            return Iterative.Done(this.Mismatch(context, first.Position, second.Errors));
                        }

                        var value = (first.Value, second.Value);
                        var length = first.Length + second.Length;
                        return Iterative.Done(this.Match(context, position, length, value));
                    });
            });
        */

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => $"Multiple({Element.ToString(depth - 1)}, {Delimiter.ToString(depth - 1)}, {Terminator.ToString(depth - 1)}, {Min}, {Max})";
}
