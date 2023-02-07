namespace Warpstone.Benchmarks.Arithmetic;

public abstract record Exp(int Precedence)
{
    protected string InnerExpToString(Exp innerExp)
    {
        if (innerExp.Precedence < Precedence)
        {
            return $"({innerExp})";
        }

        return innerExp.ToString();
    }
}

public sealed record NumExp(int Value) : Exp(4)
{
    public sealed override string ToString()
        => Value.ToString();
}

public abstract record BinOp(int Precedence, string Operator, Exp Left, Exp Right) : Exp(Precedence)
{
    public sealed override string ToString()
        => $"{InnerExpToString(Left)} {Operator} {InnerExpToString(Right)}";
}

public abstract record PrefixOp(int Precedence, string Operator, Exp Exp) : Exp(Precedence)
{
    public sealed override string ToString()
        => $"{Operator}{InnerExpToString(Exp)}";
}

public abstract record PostfixOp(int Precedence, string Operator, Exp Exp) : Exp(Precedence)
{
    public sealed override string ToString()
        => $"{InnerExpToString(Exp)}{Operator}";
}

public record AddExp(Exp Left, Exp Right) : BinOp(0, "+", Left, Right);

public record SubExp(Exp Left, Exp Right) : BinOp(0, "-", Left, Right);

public record MulExp(Exp Left, Exp Right) : BinOp(1, "*", Left, Right);

public record DivExp(Exp Left, Exp Right) : BinOp(1, "/", Left, Right);

public record MinusExp(Exp Exp) : PrefixOp(2, "-", Exp);

public record IncrementPrefixExp(Exp Exp) : PrefixOp(2, "++", Exp);

public record DecrementPrefixExp(Exp Exp) : PrefixOp(2, "--", Exp);

public record IncrementPostfixExp(Exp Exp) : PrefixOp(3, "++", Exp);

public record DecrementPostfixExp(Exp Exp) : PrefixOp(3, "--", Exp);