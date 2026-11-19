using System.Reflection;

namespace Warpstone.Benchmarks.MiniJava;

/// <summary>Immutable list with structural equality, so ASTs from different parsers can be compared.</summary>
public sealed class Seq<T> : IEquatable<Seq<T>>
{
    private readonly T[] items;

    public Seq(IEnumerable<T> items)
        => this.items = [.. items];

    public int Count => items.Length;

    public bool Equals(Seq<T>? other)
        => other is not null && items.AsSpan().SequenceEqual(other.items);

    public override bool Equals(object? obj)
        => Equals(obj as Seq<T>);

    public override int GetHashCode()
        => items.Length;

    public override string ToString()
        => "[" + string.Join(", ", items) + "]";
}

public sealed record CompilationUnit(Seq<ClassDecl> Classes);

public sealed record ClassDecl(string Name, string? Extends, Seq<IMember> Members);

public sealed record TypeRef(string Name, bool IsArray);

public sealed record Param(TypeRef Type, string Name);

public interface IMember;

public sealed record FieldDecl(Seq<string> Modifiers, TypeRef Type, string Name, IExpr? Init) : IMember;

public sealed record MethodDecl(Seq<string> Modifiers, TypeRef ReturnType, string Name, Seq<Param> Params, Seq<IStmt> Body) : IMember;

public interface IStmt;

public sealed record BlockStmt(Seq<IStmt> Body) : IStmt;

public sealed record IfStmt(IExpr Condition, IStmt Then, IStmt? Else) : IStmt;

public sealed record WhileStmt(IExpr Condition, IStmt Body) : IStmt;

public sealed record ReturnStmt(IExpr? Value) : IStmt;

public sealed record LocalVarStmt(TypeRef Type, string Name, IExpr? Init) : IStmt;

public sealed record AssignStmt(IExpr Target, IExpr Value) : IStmt;

public sealed record ExprStmt(IExpr Expr) : IStmt;

public interface IExpr;

public sealed record IntLit(int Value) : IExpr;

public sealed record StrLit(string Value) : IExpr;

public sealed record BoolLit(bool Value) : IExpr;

public sealed record NullLit : IExpr;

public sealed record ThisExpr : IExpr;

public sealed record NameExpr(string Name) : IExpr;

public sealed record BinaryExpr(string Op, IExpr Left, IExpr Right) : IExpr;

public sealed record UnaryExpr(string Op, IExpr Operand) : IExpr;

public sealed record CallExpr(IExpr? Target, string Method, Seq<IExpr> Args) : IExpr;

public sealed record FieldExpr(IExpr Target, string Name) : IExpr;

public sealed record IndexExpr(IExpr Target, IExpr Index) : IExpr;

public sealed record NewObjectExpr(string Class, Seq<IExpr> Args) : IExpr;

public sealed record NewArrayExpr(string ElementType, IExpr Size) : IExpr;

/// <summary>Grammar facts and inputs shared by all MiniJava parsers.</summary>
public static class MiniJavaGrammar
{
    /// <summary>Words that can't be identifiers.</summary>
    public static readonly HashSet<string> Keywords =
    [
        "class", "extends", "public", "private", "protected", "static", "final",
        "if", "else", "while", "return", "new", "this", "true", "false", "null",
        "int", "boolean", "void",
    ];

    public static readonly string[] Modifiers = ["public", "private", "protected", "static", "final"];

    public static readonly string[] BaseTypes = ["int", "boolean", "void"];

    /// <summary>Binary operators from lowest to highest precedence. Longer operators come first within a level.</summary>
    public static readonly string[][] BinaryOperators =
    [
        ["||"],
        ["&&"],
        ["==", "!="],
        ["<=", ">=", "<", ">"],
        ["+", "-"],
        ["*", "/", "%"],
    ];

    /// <summary>Regex that matches an identifier but no keyword.</summary>
    public static readonly string IdentifierPattern
        = $@"(?!(?:{string.Join("|", Keywords)})\b)[A-Za-z_][A-Za-z0-9_]*";

    public static bool IsIdentifierStart(char c)
        => char.IsAsciiLetter(c) || c == '_';

    public static bool IsIdentifierPart(char c)
        => char.IsAsciiLetterOrDigit(c) || c == '_';

    public static IExpr Fold(IExpr primary, IEnumerable<Func<IExpr, IExpr>> suffixes)
        => suffixes.Aggregate(primary, (e, suffix) => suffix(e));

    public static IExpr Fold(IExpr first, IEnumerable<(string Op, IExpr Right)> rest)
        => rest.Aggregate(first, (l, x) => new BinaryExpr(x.Op, l, x.Right));

    public static string LoadInput(string size)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"MiniJava.{size}.java")
            ?? throw new InvalidOperationException($"No MiniJava input '{size}'.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
