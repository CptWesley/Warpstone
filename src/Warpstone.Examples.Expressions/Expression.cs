using System;

namespace Warpstone.Examples.Expressions
{
    public abstract class Expression : ISourcePosition
    {
        public int Start { get; set; }
        public int Length { get; set; }

        public abstract int Evaluate();
    }

    public class NumExpression : Expression
    {
        public NumExpression(int value)
            => Value = value;
        
        public int Value { get; }

        public override string ToString()
            => Value.ToString();

        public override int Evaluate()
            => Value;
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

        public override int Evaluate()
            => Left.Evaluate() + Right.Evaluate();
    }

    public class SubExpression : BinaryExpression
    {
        public SubExpression(Expression left, Expression right) : base(left, right, "-")
        {
        }

        public override int Evaluate()
            => Left.Evaluate() - Right.Evaluate();
    }

    public class MulExpression : BinaryExpression
    {
        public MulExpression(Expression left, Expression right) : base(left, right, "*")
        {
        }

        public override int Evaluate()
            => Left.Evaluate() * Right.Evaluate();
    }

    public class DivExpression : BinaryExpression
    {
        public DivExpression(Expression left, Expression right) : base(left, right, "/")
        {
        }

        public override int Evaluate()
            => Left.Evaluate() / Right.Evaluate();
    }

    public class PowExpression : BinaryExpression
    {
        public PowExpression(Expression left, Expression right) : base(left, right, "^")
        {
        }

        public override int Evaluate()
        {
            double v = Math.Pow(Left.Evaluate(), Right.Evaluate());

            int r = (int)v;

            return r;
        }
    }
}
