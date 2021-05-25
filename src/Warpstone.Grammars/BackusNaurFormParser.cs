using System;
using System.Collections.Generic;
using System.Linq;
using static Warpstone.Parsers.BasicParsers;
using static Warpstone.Parsers.CommonParsers;

namespace Warpstone.Grammars
{
    /// <summary>
    /// Creates parsers for BNF grammars.
    /// </summary>
    /// <seealso cref="IGrammarParser" />
    public class BackusNaurFormParser : IGrammarParser
    {
        private static readonly IParser<string> Spaces
            = Regex("[ \t]*");

        private static readonly IParser<BnfTerm> Literal
            = Trim(Char('"').Then(Regex("[^\"]+\"").Transform(x => new BnfLiteral(x.Substring(0, x.Length - 1)) as BnfTerm)));

        private static readonly IParser<BnfTerm> Symbol
            = Trim(Char('<').Then(Regex("[a-zA-Z-]+").Transform(x => new BnfSymbol(x) as BnfTerm)).ThenSkip(Char('>')));

        private static readonly IParser<BnfTerm> Term
            = Or(Literal, Symbol);

        private static readonly IParser<IEnumerable<BnfTerm>> Sequence
            = OneOrMore(Term);

        private static readonly IParser<IEnumerable<IEnumerable<BnfTerm>>> Expression
            = OneOrMore(Sequence, Char('|'));

        private static readonly IParser<BnfRule> Rule
            = Symbol.ThenSkip(String("::=", StringComparison.InvariantCulture)).ThenAdd(Expression)
            .Transform((s, e) => new BnfRule((BnfSymbol)s, e));

        private static readonly IParser<IEnumerable<BnfRule>> Rules
            = Many(Newline).Then(Many(Rule, OneOrMore(Newline))).ThenSkip(Many(Newline)).ThenEnd();

        /// <inheritdoc/>
        public IParser<AstNode> CreateParser(string grammar)
        {
            BnfRule[] rules = Rules.Parse(grammar).ToArray();
            Dictionary<string, IParser<AstNode>> map = new Dictionary<string, IParser<AstNode>>();
            IParser<AstNode>[] parsers = new Parser<AstNode>[rules.Length];

            for (int i = 0; i < rules.Length; i++)
            {
                string name = rules[i].Symbol.Name;
                int index = i;
                map.Add(rules[i].Symbol.Name, Lazy(() => parsers[index]));
            }

            for (int i = 0; i < parsers.Length; i++)
            {
                parsers[i] = CreateParser(rules[i], map);
            }

            return parsers[0];
        }

        private static IParser<AstNode> CreateParser(BnfRule rule, Dictionary<string, IParser<AstNode>> parsers)
            => CreateParser(rule.Symbol.Name, rule.Expression, parsers);

        private static IParser<AstNode> CreateParser(string type, IEnumerable<IEnumerable<BnfTerm>> expression, Dictionary<string, IParser<AstNode>> parsers)
        {
            if (expression.Count() == 1)
            {
                return CreateParser(type, expression.First(), parsers);
            }

            return Or(CreateParser(type, expression.First(), parsers), CreateParser(type, expression.Skip(1), parsers));
        }

        private static IParser<AstNode> CreateParser(string type, IEnumerable<BnfTerm> sequence, Dictionary<string, IParser<AstNode>> parsers)
        {
            IEnumerable<IParser<AstNode>> children = sequence.Select(x => CreateParser(x, parsers));
            IParser<IEnumerable<AstNode>> result = children.First().Transform(x => new[] { x } as IEnumerable<AstNode>);
            foreach (IParser<AstNode> child in children.Skip(1))
            {
                result = result.ThenAdd(child).Transform((l, e) => l.Append(e));
            }

            return result.Transform(x => new ObjectNode(type, x) as AstNode);
        }

        private static IParser<AstNode> CreateParser(BnfTerm term, Dictionary<string, IParser<AstNode>> parsers)
        {
            if (term is BnfLiteral literal)
            {
                return String(literal.Value, StringComparison.InvariantCulture).Transform(x => new StringNode(x) as AstNode);
            }

            BnfSymbol symbol = term as BnfSymbol;
            return parsers[symbol.Name];
        }

        private static IParser<T> Trim<T>(IParser<T> parser)
            => Spaces.Then(parser).ThenSkip(Spaces);

        private abstract class BnfTerm : IParsed
        {
            public SourcePosition Position { get; set; }
        }

        private class BnfLiteral : BnfTerm
        {
            public BnfLiteral(string str)
                => Value = str;

            public string Value { get; }
        }

        private class BnfSymbol : BnfTerm
        {
            public BnfSymbol(string name)
                => Name = name;

            public string Name { get; }
        }

        private class BnfRule
        {
            public BnfRule(BnfSymbol symbol, IEnumerable<IEnumerable<BnfTerm>> expression)
            {
                Symbol = symbol;
                Expression = expression;
            }

            public BnfSymbol Symbol { get; }

            public IEnumerable<IEnumerable<BnfTerm>> Expression { get; }
        }
    }
}
