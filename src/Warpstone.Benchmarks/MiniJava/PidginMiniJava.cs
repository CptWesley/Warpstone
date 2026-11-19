using System.Globalization;
using Pidgin;
using Pidgin.Expression;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using G = Warpstone.Benchmarks.MiniJava.MiniJavaGrammar;

namespace Warpstone.Benchmarks.MiniJava;

using MemberTail = Func<Seq<string>, TypeRef, string, IMember>;
using Suffix = Func<IExpr, IExpr>;

/// <summary>MiniJava in Pidgin. Binary operators use Pidgin's <see cref="ExpressionParser"/>.</summary>
public static class PidginMiniJava
{
    private static readonly Parser<char, string> Word
        = Token(G.IsIdentifierStart).Then(Token(G.IsIdentifierPart).ManyString(), (h, t) => h + t);

    public static readonly Parser<char, CompilationUnit> Parser = Build();

    private static Parser<char, T> Tok<T>(Parser<char, T> parser)
        => parser.Before(SkipWhitespaces);

    // Pidgin doesn't backtrack after consuming input, so multi-character tokens are wrapped in Try.
    private static Parser<char, string> Sym(string symbol)
        => Tok(symbol.Length == 1 ? Char(symbol[0]).ThenReturn(symbol) : Try(String(symbol)));

    private static Parser<char, string> Kw(string keyword)
        => Tok(Try(Word.Assert(w => w == keyword)));

    private static Parser<char, CompilationUnit> Build()
    {
        Parser<char, IExpr>? expr = null;
        Parser<char, IStmt>? stmt = null;
        Parser<char, IExpr>? unary = null;
        var exprRef = Rec(() => expr!);
        var stmtRef = Rec(() => stmt!);

        var id = Tok(Try(Word.Assert(w => !G.Keywords.Contains(w))));
        var type = Map(
            (name, array) => new TypeRef(name, array.HasValue),
            OneOf(G.BaseTypes.Select(Kw).Append(id)),
            Try(Sym("[").Then(Sym("]"))).Optional());
        var args = exprRef.Separated(Sym(",")).Between(Sym("("), Sym(")")).Select(xs => new Seq<IExpr>(xs));

        var primary = OneOf(
            Tok(Digit.AtLeastOnceString()).Select(s => (IExpr)new IntLit(int.Parse(s, CultureInfo.InvariantCulture))),
            Tok(AnyCharExcept('"').ManyString().Between(Char('"'))).Select(s => (IExpr)new StrLit(s)),
            Kw("true").Select(_ => (IExpr)new BoolLit(true)),
            Kw("false").Select(_ => (IExpr)new BoolLit(false)),
            Kw("null").Select(_ => (IExpr)new NullLit()),
            Kw("this").Select(_ => (IExpr)new ThisExpr()),
            Kw("new").Then(OneOf(Kw("int"), id)).Then(
                OneOf(
                    exprRef.Between(Sym("["), Sym("]")).Select(size => (Func<string, IExpr>)(t => new NewArrayExpr(t, size))),
                    args.Select(a => (Func<string, IExpr>)(t => new NewObjectExpr(t, a)))),
                (t, create) => create(t)),
            Map((n, a) => a.HasValue ? new CallExpr(null, n, a.Value) : (IExpr)new NameExpr(n), id, args.Optional()),
            exprRef.Between(Sym("("), Sym(")")));

        var suffix = OneOf(
            Sym(".").Then(Map((n, a) => a.HasValue ? (Suffix)(t => new CallExpr(t, n, a.Value)) : t => new FieldExpr(t, n), id, args.Optional())),
            exprRef.Between(Sym("["), Sym("]")).Select(i => (Suffix)(t => new IndexExpr(t, i))));
        var postfix = Map((p, s) => G.Fold(p, s), primary, suffix.Many());

        unary = OneOf(
            Map((op, e) => (IExpr)new UnaryExpr(op, e), OneOf(Sym("!"), Sym("-")), Rec(() => unary!)),
            postfix);

        expr = ExpressionParser.Build(
            unary,
            G.BinaryOperators.Reverse().Select(ops => Operator.InfixL(
                OneOf(ops.Select(op => Sym(op).Select<Func<IExpr, IExpr, IExpr>>(o => (l, r) => new BinaryExpr(o, l, r)))))));

        var block = stmtRef.Many().Between(Sym("{"), Sym("}")).Select(xs => new Seq<IStmt>(xs));
        var cond = exprRef.Between(Sym("("), Sym(")"));
        var init = Sym("=").Then(exprRef).Optional();
        stmt = OneOf(
            block.Select(b => (IStmt)new BlockStmt(b)),
            Map((c, t, e) => (IStmt)new IfStmt(c, t, e.GetValueOrDefault()), Kw("if").Then(cond), stmtRef, Kw("else").Then(stmtRef).Optional()),
            Map((c, b) => (IStmt)new WhileStmt(c, b), Kw("while").Then(cond), stmtRef),
            Kw("return").Then(exprRef.Optional()).Before(Sym(";")).Select(e => (IStmt)new ReturnStmt(e.GetValueOrDefault())),
            Try(Map((t, n, i) => (IStmt)new LocalVarStmt(t, n, i.GetValueOrDefault()), type, id, init).Before(Sym(";"))),
            Map((l, r) => r.HasValue ? new AssignStmt(l, r.Value) : (IStmt)new ExprStmt(l), exprRef, init).Before(Sym(";")));

        var modifiers = OneOf(G.Modifiers.Select(Kw)).Many().Select(xs => new Seq<string>(xs));
        var parameters = Map((t, n) => new Param(t, n), type, id).Separated(Sym(",")).Between(Sym("("), Sym(")"))
            .Select(xs => new Seq<Param>(xs));
        var memberTail = OneOf(
            Map((p, b) => (MemberTail)((m, t, n) => new MethodDecl(m, t, n, p, b)), parameters, block),
            init.Before(Sym(";")).Select(i => (MemberTail)((m, t, n) => new FieldDecl(m, t, n, i.GetValueOrDefault()))));
        var member = Map((m, t, n, tail) => tail(m, t, n), modifiers, type, id, memberTail);
        var classDecl = Map(
            (n, e, ms) => new ClassDecl(n, e.GetValueOrDefault(), new Seq<IMember>(ms)),
            Kw("class").Then(id),
            Kw("extends").Then(id).Optional(),
            member.Many().Between(Sym("{"), Sym("}")));

        return SkipWhitespaces.Then(classDecl.Many()).Before(End).Select(cs => new CompilationUnit(new Seq<ClassDecl>(cs)));
    }
}
