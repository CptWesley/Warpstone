﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Warpstone.Parsers;

/// <summary>
/// A parser which wraps a parser and performs a transformation function on the result.
/// </summary>
/// <typeparam name="TIn">The result type of the wrapped parser.</typeparam>
/// <typeparam name="TOut">The result type of the parser.</typeparam>
public class TransformParser<TIn, TOut> : Parser<TOut>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TransformParser{TInput, TOutput}"/> class.
    /// </summary>
    /// <param name="parser">The wrapped parser.</param>
    /// <param name="transformation">The transformation applied to the result of the wrapped parser.</param>
    public TransformParser(IParser<TIn> parser, Func<TIn, TOut> transformation)
    {
        Parser = parser;
        Transformation = transformation;
    }

    /// <summary>
    /// Gets the wrapped parser.
    /// </summary>
    public IParser<TIn> Parser { get; }

    /// <summary>
    /// Gets the transformation applied to the result of the wrapped parser.
    /// </summary>
    internal Func<TIn, TOut> Transformation { get; }

    /// <inheritdoc/>
    [SuppressMessage("Microsoft.Design", "CA1031", Justification = "General exception catch needed for correct behaviour.")]
    protected override IParseResult<TOut> InternalTryMatch(string input, int position, int maxLength, IMemoTable memoTable, CancellationToken cancellationToken)
    {
        IParseResult<TIn> result = Parser.TryMatch(input, position, maxLength, memoTable, cancellationToken);

        if (!result.Success)
        {
            return new ParseResult<TOut>(this, result.Error, new[] { result });
        }

        try
        {
            TOut value = Transformation(result.Value!);
            if (value is IParsed parsed && parsed.Position == default!)
            {
                parsed.Position = new SourcePosition(input, position, result.Position.End - 1);
            }

            return new ParseResult<TOut>(this, value, input, position, result.Position.End, new[] { result });
        }
        catch (Exception e)
        {
            return new ParseResult<TOut>(this, new TransformationError(new SourcePosition(input, position, result.Position.End - 1), e), new[] { result });
        }
    }

    /// <inheritdoc/>
    protected override string InternalToString(int depth)
        => $"Transform({Parser.ToString(depth - 1)}, {Transformation})";
}