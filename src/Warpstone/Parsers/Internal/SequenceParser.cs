using System.Collections.Generic;

namespace Warpstone.Parsers.Internal;

/// <summary>
/// A parser which runs two parsers in sequence where both parsers should succeed
/// in order for this parser to succeed.
/// </summary>
/// <typeparam name="TFirst">The result type of the first parser.</typeparam>
/// <typeparam name="TSecond">The result type of the second parser.</typeparam>
internal sealed class SequenceParser<TFirst, TSecond> : ParserBase<(TFirst, TSecond)>, IParserSecond<TFirst, TSecond>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SequenceParser{TFirst, TSecond}"/> class.
    /// </summary>
    /// <param name="first">The parser which is attempted first.</param>
    /// <param name="second">The parser which is attempted second.</param>
    public SequenceParser(IParser<TFirst> first, IParser<TSecond> second)
    {
        First = first;
        Second = second;
    }

    /// <summary>
    /// The parser which is attempted first.
    /// </summary>
    public IParser<TFirst> First { get; }

    /// <summary>
    /// The parser which is attempted second.
    /// </summary>
    public IParser<TSecond> Second { get; }

    /// <inheritdoc />
    public override IterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IterativeStep> eval)
        => Iterative.More(
            () => eval(First, position),
            untypedFirst =>
            {
                var first = untypedFirst.AssertOfType<IParseResult<TFirst>>();

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
                            return Iterative.Done(this.Mismatch(context, position, second.Errors));
                        }

                        var value = (first.Value, second.Value);
                        var length = first.Length + second.Length;
                        return Iterative.Done(this.Match(context, position, length, value));
                    });
            });

    public override void Eval(IReadOnlyParseContext context, int position, IParseStack stack)
    {
        stack.Push(new Choice1
        {
            Parser = this,
            Context = context,
            Position = position,
            Stack = stack,
        });
        stack.Push(new ApplyParserInstruction
        {
            Parser = First,
            Context = context,
            Position = position,
            Stack = stack,
        });
    }

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => $"({First.ToString(depth - 1)} {Second.ToString(depth - 1)})";

    private abstract class Choice : ParseInstruction
    {
        public required SequenceParser<TFirst, TSecond> Parser { get; init; }
    }

    private sealed class Choice1 : Choice
    {
        public override void Execute()
        {
            var first = (IParseResult)Stack.Last!;
            Stack.Pop();

            if (!first.Success)
            {
                Stack.Push(Parser.Mismatch(Context, Position, first.Errors));
                return;
            }

            Stack.Push(new Choice2
            {
                Parser = Parser,
                Context = Context,
                Position = Position,
                Stack = Stack,
                First = first,
            });
            Stack.Push(new ApplyParserInstruction
            {
                Parser = Parser.Second,
                Context = Context,
                Position = first.NextPosition,
                Stack = Stack,
            });
        }
    }

    private sealed class Choice2 : Choice
    {
        public required IParseResult First { get; init; }

        public override void Execute()
        {
            var second = (IParseResult)Stack.Last!;
            Stack.Pop();

            if (!second.Success)
            {
                Stack.Push(Parser.Mismatch(Context, Position, second.Errors));
                return;
            }

            var value = ((TFirst)First.Value!, (TSecond)second.Value!);
            var length = First.Length + second.Length;
            var result = Parser.Match(Context, Position, length, value);

            Stack.Push(result);
        }
    }
}
