using System.Globalization;
using Parlot;
using Parlot.Fluent;
using Parlot.SourceGenerator;
using static Parlot.Fluent.Parsers;
using G = Warpstone.Benchmarks.MiniJava.MiniJavaGrammar;

namespace Warpstone.Benchmarks.MiniJava;

using MemberTail = Func<Seq<string>, TypeRef, string, IMember>;
using Suffix = Func<IExpr, IExpr>;

// Build-only factory for ParlotGeneratedMiniJava. Its Build() is a verbatim copy of ParlotMiniJava.Build(), so both Parlot
// benchmarks parse the same grammar. Keep them in sync (verify-minijava checks that their ASTs match).
public static partial class ParlotGeneratedMiniJava
{
    // Terms skip the whitespace before a token.
    private static Parser<string> Sym(string symbol)
        => Terms.Text(symbol);

    private static Parser<string> Kw(string keyword)
        => Terms.Keyword(keyword);

    [GenerateParser(nameof(TryParse))]
    private static Parser<CompilationUnit> Build()
    {
        var expr = Deferred<IExpr>();
        var stmt = Deferred<IStmt>();
        var unary = Deferred<IExpr>();

        var id = Terms.Identifier().Then(x => x.ToString()).When((_, w) => !G.Keywords.Contains(w));
        var type = OneOf(G.BaseTypes.Select(Kw).Append(id).ToArray())
            .And(Sym("[").And(Sym("]")).Then(_ => true).Else(false))
            .Then(x => new TypeRef(x.Item1, x.Item2));
        var args = Between(Sym("("), Separated(Sym(","), expr).Else(static _ => Array.Empty<IExpr>()), Sym(")")).Then(xs => new Seq<IExpr>(xs));
        var optArgs = args.Then(static a => (Seq<IExpr>?)a).Else((Seq<IExpr>?)null);

        var primary = OneOf(
            Terms.Pattern(char.IsAsciiDigit).Then(s => (IExpr)new IntLit(int.Parse(s.Span, CultureInfo.InvariantCulture))),
            Terms.String(StringLiteralQuotes.Double).Then(s => (IExpr)new StrLit(s.ToString())),
            Kw("true").Then(_ => (IExpr)new BoolLit(true)),
            Kw("false").Then(_ => (IExpr)new BoolLit(false)),
            Kw("null").Then(_ => (IExpr)new NullLit()),
            Kw("this").Then(_ => (IExpr)new ThisExpr()),
            Kw("new").SkipAnd(Kw("int").Or(id)).And(OneOf(
                    Between(Sym("["), expr, Sym("]")).Then(size => (Func<string, IExpr>)(t => new NewArrayExpr(t, size))),
                    args.Then(a => (Func<string, IExpr>)(t => new NewObjectExpr(t, a)))))
                .Then(x => x.Item2(x.Item1)),
            id.And(optArgs).Then(x => x.Item2 is null ? new NameExpr(x.Item1) : (IExpr)new CallExpr(null, x.Item1, x.Item2)),
            Between(Sym("("), expr, Sym(")")));

        var suffix = OneOf(
            Sym(".").SkipAnd(id).And(optArgs).Then(x => x.Item2 is null ? (Suffix)(t => new FieldExpr(t, x.Item1)) : t => new CallExpr(t, x.Item1, x.Item2)),
            Between(Sym("["), expr, Sym("]")).Then(i => (Suffix)(t => new IndexExpr(t, i))));
        var postfix = primary.And(ZeroOrMany(suffix)).Then(x => G.Fold(x.Item1, x.Item2));

        unary.Parser = OneOf(
            Sym("!").Or(Sym("-")).And(unary).Then(x => (IExpr)new UnaryExpr(x.Item1, x.Item2)),
            postfix);

        Parser<IExpr> level = unary;
        foreach (var ops in G.BinaryOperators.Reverse())
        {
            level = Parlot.Fluent.Parsers.LeftAssociative(level, ops.Select(Sym).ToArray(), (l, r, op) => new BinaryExpr(op, l, r));
        }

        expr.Parser = level;

        var block = Between(Sym("{"), ZeroOrMany(stmt), Sym("}")).Then(xs => new Seq<IStmt>(xs));
        var cond = Between(Sym("("), expr, Sym(")"));
        var init = Sym("=").SkipAnd(expr).Then(static e => (IExpr?)e).Else((IExpr?)null);
        stmt.Parser = OneOf(
            block.Then(b => (IStmt)new BlockStmt(b)),
            Kw("if").SkipAnd(cond).And(stmt).And(Kw("else").SkipAnd(stmt).Then(static s => (IStmt?)s).Else((IStmt?)null)).Then(x => (IStmt)new IfStmt(x.Item1, x.Item2, x.Item3)),
            Kw("while").SkipAnd(cond).And(stmt).Then(x => (IStmt)new WhileStmt(x.Item1, x.Item2)),
            Kw("return").SkipAnd(expr.Then(static e => (IExpr?)e).Else((IExpr?)null)).AndSkip(Sym(";")).Then(e => (IStmt)new ReturnStmt(e)),
            type.And(id).And(init).AndSkip(Sym(";")).Then(x => (IStmt)new LocalVarStmt(x.Item1, x.Item2, x.Item3)),
            expr.And(init).AndSkip(Sym(";")).Then(x => x.Item2 is null ? new ExprStmt(x.Item1) : (IStmt)new AssignStmt(x.Item1, x.Item2)));

        var modifiers = ZeroOrMany(OneOf(G.Modifiers.Select(Kw).ToArray())).Then(xs => new Seq<string>(xs));
        var parameters = Between(Sym("("), Separated(Sym(","), type.And(id).Then(x => new Param(x.Item1, x.Item2))).Else(static _ => Array.Empty<Param>()), Sym(")"))
            .Then(xs => new Seq<Param>(xs));
        var memberTail = OneOf(
            parameters.And(block).Then(x => (MemberTail)((m, t, n) => new MethodDecl(m, t, n, x.Item1, x.Item2))),
            init.AndSkip(Sym(";")).Then(i => (MemberTail)((m, t, n) => new FieldDecl(m, t, n, i))));
        var member = modifiers.And(type).And(id).And(memberTail).Then(x => x.Item4(x.Item1, x.Item2, x.Item3));
        var classDecl = Kw("class").SkipAnd(id).And(Kw("extends").SkipAnd(id).Then(static n => (string?)n).Else((string?)null)).AndSkip(Sym("{")).And(ZeroOrMany(member)).AndSkip(Sym("}"))
            .Then(x => new ClassDecl(x.Item1, x.Item2, new Seq<IMember>(x.Item3)));

        return ZeroOrMany(classDecl).AndSkip(ZeroOrOne(Literals.WhiteSpace(true))).Eof()
            .Then(cs => new CompilationUnit(new Seq<ClassDecl>(cs)));
    }
}
