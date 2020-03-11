namespace Warpstone.Examples.Expressions
{
    public abstract class Expression : ISourcePosition
    {
        public int Start { get; set; }
        public int Length { get; set; }
    }

    public class NumExpression : Expression
    {
        public NumExpression(int value)
            => Value = value;

        public int Value { get; }

        public override string ToString()
            => Value.ToString();
    }

    public abstract class BinaryExpression : Expression
    {
        public BinaryExpression(Expression left, Expression right, string op)
        {
            Left = left;
            Right = right;
            Operator = op;
        }

        public Expression Left { get; }
        public Expression Right { get; }

        public string Operator { get; }

        public override string ToString()
            => $"({Left} {Operator} {Right})";
    }

    public class AddExpression : BinaryExpression
    {
        public AddExpression(Expression left, Expression right) : base(left, right, "+")
        {
        }
    }

    public class MulExpression : BinaryExpression
    {
        public MulExpression(Expression left, Expression right) : base(left, right, "*")
        {
        }
    }
}
