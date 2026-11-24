using System.Globalization;
using static Warpstone.Parsers;
using G = Warpstone.Benchmarks.MiniJava.MiniJavaGrammar;

namespace Warpstone.Benchmarks.MiniJava;

using MemberTail = Func<Seq<string>, TypeRef, string, IMember>;
using Suffix = Func<IExpr, IExpr>;

/// <summary>MiniJava in Warpstone 3.</summary>
public static class Warpstone3MiniJava
{
    /// <summary>Binary operators as <c>operand (op operand)*</c>, folded to the left, like the other libraries.</summary>
    public static readonly IParser<CompilationUnit> Parser = Build(leftRecursive: false);

    /// <summary>Binary operators as left-recursive rules (<c>e := e op operand | operand</c>).</summary>
    public static readonly IParser<CompilationUnit> LeftRecursiveParser = Build(leftRecursive: true);

    /// <summary>
    /// The namespaces and aliases the transformations use, for <see cref="ParserExtensions.GenerateSource{T}"/>.
    /// Aliases can't use the other usings, so their targets are fully qualified.
    /// </summary>
    public static readonly string[] Usings =
    [
        "System.Globalization",
        "G = Warpstone.Benchmarks.MiniJava.MiniJavaGrammar",
        "MemberTail = System.Func<Warpstone.Benchmarks.MiniJava.Seq<string>, Warpstone.Benchmarks.MiniJava.TypeRef, string, Warpstone.Benchmarks.MiniJava.IMember>",
        "Suffix = System.Func<Warpstone.Benchmarks.MiniJava.IExpr, Warpstone.Benchmarks.MiniJava.IExpr>",
    ];

    /// <summary>
    /// Writes the generated parsers to <c>Warpstone3GeneratedMiniJava.g.cs</c> and <c>Warpstone3GeneratedLeftRecursiveMiniJava.g.cs</c> in <paramref name="directory"/>.
    /// </summary>
    /// <param name="directory">The directory, normally <c>Warpstone.Benchmarks/MiniJava</c>.</param>
    public static void Generate(string directory)
    {
        const string Namespace = "Warpstone.Benchmarks.MiniJava";
        File.WriteAllText(Path.Combine(directory, "Warpstone3GeneratedMiniJava.g.cs"), Parser.GenerateSource(Namespace, "Warpstone3GeneratedMiniJava", Usings));
        File.WriteAllText(Path.Combine(directory, "Warpstone3GeneratedLeftRecursiveMiniJava.g.cs"), LeftRecursiveParser.GenerateSource(Namespace, "Warpstone3GeneratedLeftRecursiveMiniJava", Usings));
    }

    private static IParser<T> Tok<T>(IParser<T> parser)
        => parser.ThenSkip(OptionalWhitespaces);

    private static IParser<string> Sym(string symbol)
        => Tok(String(symbol));

    private static IParser<string> Kw(string keyword)
        => Tok(Regex(keyword + @"\b"));

    private static IParser<IExpr> Chain(IParser<IExpr> operand, string[] ops)
        => operand
            .ThenAdd(Many(Or(ops.Select(Sym).ToArray()).ThenAdd(operand)))
            .Transform((first, rest) => G.Fold(first, rest));

    private static IParser<IExpr> LeftRecursive(IParser<IExpr> operand, string[] ops)
    {
        IParser<IExpr>? self = null;
        self = Or(
            Lazy(() => self!).ThenAdd(Or(ops.Select(Sym).ToArray())).ThenAdd(operand).Transform((l, op, r) => (IExpr)new BinaryExpr(op, l, r)),
            operand);
        return self;
    }

    private static IParser<CompilationUnit> Build(bool leftRecursive)
    {
        IParser<IExpr>? expr = null;
        IParser<IStmt>? stmt = null;
        IParser<IExpr>? unary = null;
        var exprRef = Lazy(() => expr!);
        var stmtRef = Lazy(() => stmt!);

        var id = Tok(Regex(G.IdentifierPattern));
        var type = Or(G.BaseTypes.Select(Kw).Append(id).ToArray())
            .ThenAdd(Maybe(Sym("[").Then(Sym("]")).Transform(_ => true), false))
            .Transform((name, isArray) => new TypeRef(name, isArray));
        var args = Sym("(").Then(Many(exprRef, Sym(","))).ThenSkip(Sym(")")).Transform(xs => new Seq<IExpr>(xs));

        var primary = Or(
            Tok(Regex("[0-9]+")).Transform(s => (IExpr)new IntLit(int.Parse(s, CultureInfo.InvariantCulture))),
            Tok(Regex("\"[^\"]*\"")).Transform(s => (IExpr)new StrLit(s[1..^1])),
            Kw("true").Transform(_ => (IExpr)new BoolLit(true)),
            Kw("false").Transform(_ => (IExpr)new BoolLit(false)),
            Kw("null").Transform(_ => (IExpr)new NullLit()),
            Kw("this").Transform(_ => (IExpr)new ThisExpr()),
            Kw("new").Then(Or(Kw("int"), id)).ThenAdd(Or(
                    Sym("[").Then(exprRef).ThenSkip(Sym("]")).Transform(size => (Func<string, IExpr>)(t => new NewArrayExpr(t, size))),
                    args.Transform(a => (Func<string, IExpr>)(t => new NewObjectExpr(t, a)))))
                .Transform((t, create) => create(t)),
            id.ThenAdd(Maybe(args)).Transform((n, a) => a is null ? new NameExpr(n) : (IExpr)new CallExpr(null, n, a)),
            Sym("(").Then(exprRef).ThenSkip(Sym(")")));

        var suffix = Or(
            Sym(".").Then(id).ThenAdd(Maybe(args)).Transform((n, a) => a is null ? (Suffix)(t => new FieldExpr(t, n)) : t => new CallExpr(t, n, a)),
            Sym("[").Then(exprRef).ThenSkip(Sym("]")).Transform(i => (Suffix)(t => new IndexExpr(t, i))));
        var postfix = primary.ThenAdd(Many(suffix)).Transform((p, s) => G.Fold(p, s));

        unary = Or(
            Or(Sym("!"), Sym("-")).ThenAdd(Lazy(() => unary!)).Transform((op, e) => (IExpr)new UnaryExpr(op, e)),
            postfix);

        var level = unary;
        foreach (var ops in G.BinaryOperators.Reverse())
        {
            level = leftRecursive ? LeftRecursive(level, ops) : Chain(level, ops);
        }

        expr = level;

        var block = Sym("{").Then(Many(stmtRef)).ThenSkip(Sym("}")).Transform(xs => new Seq<IStmt>(xs));
        var cond = Sym("(").Then(exprRef).ThenSkip(Sym(")"));
        var init = Maybe(Sym("=").Then(exprRef));
        stmt = Or(
            block.Transform(b => (IStmt)new BlockStmt(b)),
            Kw("if").Then(cond).ThenAdd(stmtRef).ThenAdd(Maybe(Kw("else").Then(stmtRef))).Transform((c, t, e) => (IStmt)new IfStmt(c, t, e)),
            Kw("while").Then(cond).ThenAdd(stmtRef).Transform((c, b) => (IStmt)new WhileStmt(c, b)),
            Kw("return").Then(Maybe(exprRef)).ThenSkip(Sym(";")).Transform(e => (IStmt)new ReturnStmt(e)),
            type.ThenAdd(id).ThenAdd(init).ThenSkip(Sym(";")).Transform((t, n, i) => (IStmt)new LocalVarStmt(t, n, i)),
            exprRef.ThenAdd(Maybe(Sym("=").Then(exprRef))).ThenSkip(Sym(";")).Transform((l, r) => r is null ? new ExprStmt(l) : (IStmt)new AssignStmt(l, r)));

        var modifiers = Many(Or(G.Modifiers.Select(Kw).ToArray())).Transform(xs => new Seq<string>(xs));
        var parameters = Sym("(").Then(Many(type.ThenAdd(id).Transform((t, n) => new Param(t, n)), Sym(","))).ThenSkip(Sym(")"))
            .Transform(xs => new Seq<Param>(xs));
        var memberTail = Or(
            parameters.ThenAdd(block).Transform((p, b) => (MemberTail)((m, t, n) => new MethodDecl(m, t, n, p, b))),
            init.ThenSkip(Sym(";")).Transform(i => (MemberTail)((m, t, n) => new FieldDecl(m, t, n, i))));
        var member = modifiers.ThenAdd(type).ThenAdd(id).ThenAdd(memberTail).Transform((m, t, n, tail) => tail(m, t, n));
        var classDecl = Kw("class").Then(id).ThenAdd(Maybe(Kw("extends").Then(id))).ThenSkip(Sym("{")).ThenAdd(Many(member)).ThenSkip(Sym("}"))
            .Transform((n, e, ms) => new ClassDecl(n, e, new Seq<IMember>(ms)));

        return OptionalWhitespaces.Then(Many(classDecl)).ThenEnd().Transform(cs => new CompilationUnit(new Seq<ClassDecl>(cs)));
    }
}
