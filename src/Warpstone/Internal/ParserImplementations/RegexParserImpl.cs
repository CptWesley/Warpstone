using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Warpstone.Errors;
using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations
{
    /// <summary>
    /// Represents a parser that matches regular expressions in the input.
    /// </summary>
    internal sealed class RegexParserImpl : ParserImplementationBase<RegexParser, string>
    {
        private readonly string expected;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexParserImpl"/> class.
        /// </summary>
        /// <param name="pattern">The pattern to be matched.</param>
        /// <param name="options">The options used by the regex engine.</param>
        /// <param name="timeout">The timeout for the regex engine.</param>
        public RegexParserImpl([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, RegexOptions options, TimeSpan timeout)
        {
            var o = options | RegexOptions.ExplicitCapture;
            expected = Regex.Escape(pattern);
            if (timeout == TimeSpan.Zero || timeout == TimeSpan.MaxValue)
            {
                Regex = new Regex(@$"\G({pattern})", o);
            }
            else
            {
                Regex = new Regex(@$"\G({pattern})", o, timeout);
            }
        }

        /// <summary>
        /// The regular expression engine.
        /// </summary>
        public Regex Regex { get; }

        /// <inheritdoc />
        protected override void InitializeInternal(RegexParser parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
        {
            // Do nothing.
        }

        /// <inheritdoc />
        public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
        {
            if (ReaderParseInput.RequestMore(context.Input, int.MaxValue))
            {
                return Apply(context, position);
            }

            var input = context.Input.Content;
            var match = Regex.Match(input, position);

            if (match.Success && match.Index == position)
            {
                return new(position, match.Length, match.Value);
            }
            else
            {
                return new(position, new UnexpectedTokenError(context, this, position, 1, expected));
            }
        }

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            if (ReaderParseInput.RequestMore(context.Input, int.MaxValue))
            {
                context.ExecutionStack.Push((position, this));
                return;
            }

            var input = context.Input.Content;
            var match = Regex.Match(input, position);

            if (match.Success && match.Index == position)
            {
                context.ResultStack.Push(new(position, match.Length, match.Value));
            }
            else
            {
                context.ResultStack.Push(new(position, new UnexpectedTokenError(context, this, position, 1, expected)));
            }
        }
    }
}
