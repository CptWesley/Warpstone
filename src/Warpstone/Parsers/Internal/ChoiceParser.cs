using DotNetProjectFile.Parsing;
using System.Collections.Generic;

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
    public override IterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IterativeStep> eval)
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

    private abstract class Choice : ParseInstruction
    {
        public required ChoiceParser<T> Parser { get; init; }
    }

    private sealed class Choice1 : Choice
    {
        public override void Execute()
        {
            var first = (IParseResult)Stack.Last!;

            if (first.Success)
            {
                return;
            }

            Stack.Pop();

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
                Position = Position,
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

            if (second.Success)
            {
                return;
            }

            Stack.Pop();

            var errors = Parser.MergeErrors(First.Errors, second.Errors);
            var result = Parser.Mismatch(Context, Position, errors);

            Stack.Push(result);
        }
    }
}
