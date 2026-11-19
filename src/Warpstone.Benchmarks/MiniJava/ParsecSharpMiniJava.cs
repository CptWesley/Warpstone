using System.Globalization;
using ParsecSharp;
using static ParsecSharp.Parser;
using static ParsecSharp.Text;
using G = Warpstone.Benchmarks.MiniJava.MiniJavaGrammar;

namespace Warpstone.Benchmarks.MiniJava;

using MemberTail = Func<Seq<string>, TypeRef, string, IMember>;
using Suffix = Func<IExpr, IExpr>;

/// <summary>MiniJava in ParsecSharp. Binary operators use <c>ChainLeft</c>.</summary>
public static class ParsecSharpMiniJava
{
    private static readonly IParser<char, string> Word
        = Sequence(Satisfy(G.IsIdentifierStart), Many(Satisfy(G.IsIdentifierPart)).AsString(), (h, t) => h + t);

    public static readonly IParser<char, CompilationUnit> Parser = Build();

    private static IParser<char, T> Tok<T>(IParser<char, T> parser)
        => parser.Left(Spaces());

    private static IParser<char, string> Sym(string symbol)
        => Tok(String(symbol));

    private static IParser<char, string> Kw(string keyword)
        => Tok(Word.Guard(w => w == keyword));

    private static IParser<char, T?> Opt<T>(IParser<char, T> parser)
        where T : class
        => Optional(parser.Map(x => (T?)x), null);

    private static IParser<char, CompilationUnit> Build()
    {
        IParser<char, IExpr>? expr = null;
        IParser<char, IStmt>? stmt = null;
        IParser<char, IExpr>? unary = null;
        var exprRef = Delay(() => expr!);
        var stmtRef = Delay(() => stmt!);

        var id = Tok(Word.Guard(w => !G.Keywords.Contains(w)));
        var type = Sequence(
            Choice(G.BaseTypes.Select(Kw).Append(id)),
            Optional(Sym("[").Right(Sym("]"))),
            (name, array) => new TypeRef(name, array));
        var args = exprRef.SeparatedBy(Sym(",")).Between(Sym("("), Sym(")")).Map(xs => new Seq<IExpr>(xs));

        var primary = Choice(
            Tok(Many1(DecDigit()).AsString()).Map(s => (IExpr)new IntLit(int.Parse(s, CultureInfo.InvariantCulture))),
            Tok(Many(NoneOf("\"")).AsString().Between(Char('"'))).Map(s => (IExpr)new StrLit(s)),
            Kw("true").Map(_ => (IExpr)new BoolLit(true)),
            Kw("false").Map(_ => (IExpr)new BoolLit(false)),
            Kw("null").Map(_ => (IExpr)new NullLit()),
            Kw("this").Map(_ => (IExpr)new ThisExpr()),
            Sequence(
                Kw("new").Right(Choice(Kw("int"), id)),
                Choice(
                    exprRef.Between(Sym("["), Sym("]")).Map(size => (Func<string, IExpr>)(t => new NewArrayExpr(t, size))),
                    args.Map(a => (Func<string, IExpr>)(t => new NewObjectExpr(t, a)))),
                (t, create) => create(t)),
            Sequence(id, Opt(args), (n, a) => a is null ? new NameExpr(n) : (IExpr)new CallExpr(null, n, a)),
            exprRef.Between(Sym("("), Sym(")")));

        var suffix = Choice(
            Sequence(Sym(".").Right(id), Opt(args), (n, a) => a is null ? (Suffix)(t => new FieldExpr(t, n)) : t => new CallExpr(t, n, a)),
            exprRef.Between(Sym("["), Sym("]")).Map(i => (Suffix)(t => new IndexExpr(t, i))));
        var postfix = Sequence(primary, Many(suffix), (p, s) => G.Fold(p, s));

        unary = Choice(
            Sequence(Choice(Sym("!"), Sym("-")), Delay(() => unary!), (op, e) => (IExpr)new UnaryExpr(op, e)),
            postfix);

        var level = unary;
        foreach (var ops in G.BinaryOperators.Reverse())
        {
            level = level.ChainLeft(Choice(ops.Select(op => Sym(op).Map<char, string, Func<IExpr, IExpr, IExpr>>(o => (l, r) => new BinaryExpr(o, l, r)))));
        }

        expr = level;

        var block = Many(stmtRef).Between(Sym("{"), Sym("}")).Map(xs => new Seq<IStmt>(xs));
        var cond = exprRef.Between(Sym("("), Sym(")"));
        var init = Opt(Sym("=").Right(exprRef));
        stmt = Choice(
            block.Map(b => (IStmt)new BlockStmt(b)),
            Sequence(Kw("if").Right(cond), stmtRef, Opt(Kw("else").Right(stmtRef)), (c, t, e) => (IStmt)new IfStmt(c, t, e)),
            Sequence(Kw("while").Right(cond), stmtRef, (c, b) => (IStmt)new WhileStmt(c, b)),
            Kw("return").Right(Opt(exprRef)).Left(Sym(";")).Map(e => (IStmt)new ReturnStmt(e)),
            Sequence(type, id, init, (t, n, i) => (IStmt)new LocalVarStmt(t, n, i)).Left(Sym(";")),
            Sequence(exprRef, init, (l, r) => r is null ? new ExprStmt(l) : (IStmt)new AssignStmt(l, r)).Left(Sym(";")));

        var modifiers = Many(Choice(G.Modifiers.Select(Kw))).Map(xs => new Seq<string>(xs));
        var parameters = Sequence(type, id, (t, n) => new Param(t, n)).SeparatedBy(Sym(",")).Between(Sym("("), Sym(")"))
            .Map(xs => new Seq<Param>(xs));
        var memberTail = Choice(
            Sequence(parameters, block, (p, b) => (MemberTail)((m, t, n) => new MethodDecl(m, t, n, p, b))),
            init.Left(Sym(";")).Map(i => (MemberTail)((m, t, n) => new FieldDecl(m, t, n, i))));
        var member = Sequence(modifiers, type, id, memberTail, (m, t, n, tail) => tail(m, t, n));
        var classDecl = Sequence(
            Kw("class").Right(id),
            Opt(Kw("extends").Right(id)),
            Many(member).Between(Sym("{"), Sym("}")),
            (n, e, ms) => new ClassDecl(n, e, new Seq<IMember>(ms)));

        return Spaces().Right(Many(classDecl)).Left(EndOfInput()).Map(cs => new CompilationUnit(new Seq<ClassDecl>(cs)));
    }
}
