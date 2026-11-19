using System.Globalization;
using Farkle;
using Farkle.Builder;
using G = Warpstone.Benchmarks.MiniJava.MiniJavaGrammar;
using ParserState = Farkle.Parser.ParserState;
using Regex = Farkle.Builder.Regex;

namespace Warpstone.Benchmarks.MiniJava;

using MemberTail = Func<Seq<string>, TypeRef, string, IMember>;

/// <summary>
/// MiniJava in Farkle, an LALR(1) parser generator. The parsing tables are built at runtime, once, when this class is
/// initialized. Binary operators are left-recursive precedence levels, and the dangling <c>else</c> is resolved by
/// splitting statements into closed and open ones, so the grammar has no conflicts.
/// </summary>
public static class FarkleMiniJava
{
    public static readonly CharParser<CompilationUnit> Parser = Build();

    public static CompilationUnit Parse(string input)
    {
        var result = Parser.Parse(input);
        return result.IsSuccess ? result.Value : throw new InvalidOperationException(result.Error?.ToString());
    }

    private static CharParser<CompilationUnit> Build()
    {
        // Literals ("if", "int", ...) take priority over the identifier regex, which keeps keywords out of identifiers.
        var id = Terminal.Create(
            "Identifier",
            Regex.FromRegexString("[A-Za-z_][A-Za-z0-9_]*"),
            (ref ParserState _, ReadOnlySpan<char> s) => s.ToString());
        var intLit = Terminal.Create(
            "Integer",
            Regex.FromRegexString("[0-9]+"),
            (ref ParserState _, ReadOnlySpan<char> s) => (IExpr)new IntLit(int.Parse(s, CultureInfo.InvariantCulture)));
        var strLit = Terminal.Create(
            "String",
            Regex.FromRegexString("\"[^\"]*\""),
            (ref ParserState _, ReadOnlySpan<char> s) => (IExpr)new StrLit(s[1..^1].ToString()));

        // "[]" is one token, so that "a[" (indexing) and "int[]" (array type) don't need two tokens of lookahead.
        var arrayBrackets = Terminal.Create("ArrayBrackets", Regex.Literal("[") + Regex.FromRegexString(@"\s*") + Regex.Literal("]"));

        var expr = Nonterminal.Create<IExpr>("Expression");
        var unary = Nonterminal.Create<IExpr>("Unary");
        var postfix = Nonterminal.Create<IExpr>("Postfix");
        var stmt = Nonterminal.Create<IStmt>("Statement");
        var closed = Nonterminal.Create<IStmt>("ClosedStatement");
        var open = Nonterminal.Create<IStmt>("OpenStatement");

        var baseType = Nonterminal.Create<string>(
            "BaseType",
            [.. G.BaseTypes.Select(t => t.FinishConstant(t)), id.AsProduction()]);
        var type = Nonterminal.Create(
            "Type",
            baseType.Finish(n => new TypeRef(n, false)),
            baseType.Extended().Append(arrayBrackets).Finish(n => new TypeRef(n, true)));
        var args = Nonterminal.Create(
            "Arguments",
            "(".Appended().Extend(expr.SeparatedBy<IExpr, List<IExpr>>(Terminal.Literal(","))).Append(")").Finish(xs => new Seq<IExpr>(xs)));

        var primary = Nonterminal.Create(
            "Primary",
            intLit.AsProduction(),
            strLit.AsProduction(),
            "true".Finish(() => (IExpr)new BoolLit(true)),
            "false".Finish(() => (IExpr)new BoolLit(false)),
            "null".Finish(() => (IExpr)new NullLit()),
            "this".Finish(() => (IExpr)new ThisExpr()),
            "new".Appended().Append("int").Append("[").Extend(expr).Append("]").Finish(size => (IExpr)new NewArrayExpr("int", size)),
            "new".Appended().Extend(id).Append("[").Extend(expr).Append("]").Finish((t, size) => (IExpr)new NewArrayExpr(t, size)),
            "new".Appended().Extend(id).Extend(args).Finish((t, a) => (IExpr)new NewObjectExpr(t, a)),
            id.Finish(n => (IExpr)new NameExpr(n)),
            id.Extended().Extend(args).Finish((n, a) => (IExpr)new CallExpr(null, n, a)),
            "(".Appended().Extend(expr).Append(")").AsProduction());

        postfix.SetProductions(
            primary.AsProduction(),
            postfix.Extended().Append(".").Extend(id).Finish((t, n) => (IExpr)new FieldExpr(t, n)),
            postfix.Extended().Append(".").Extend(id).Extend(args).Finish((t, n, a) => (IExpr)new CallExpr(t, n, a)),
            postfix.Extended().Append("[").Extend(expr).Append("]").Finish((t, i) => (IExpr)new IndexExpr(t, i)));

        unary.SetProductions(
            "!".Appended().Extend(unary).Finish(e => (IExpr)new UnaryExpr("!", e)),
            "-".Appended().Extend(unary).Finish(e => (IExpr)new UnaryExpr("-", e)),
            postfix.AsProduction());

        IGrammarSymbol<IExpr> level = unary;
        var depth = 0;
        foreach (var ops in G.BinaryOperators.Reverse())
        {
            var operand = level;
            var binary = Nonterminal.Create<IExpr>($"Binary{depth++}");
            binary.SetProductions(
            [
                operand.AsProduction(),
                .. ops.Select(op => binary.Extended().Append(op).Extend(operand).Finish((l, r) => (IExpr)new BinaryExpr(op, l, r))),
            ]);
            level = binary;
        }

        expr.SetProductions(level.AsProduction());

        var block = Nonterminal.Create(
            "Block",
            "{".Appended().Extend(stmt.Many<IStmt, List<IStmt>>()).Append("}").Finish(xs => new Seq<IStmt>(xs)));
        var init = Nonterminal.Create("Initializer", "=".Appended().Extend(expr).AsProduction()).Optional();
        var simple = Nonterminal.Create(
            "SimpleStatement",
            block.Finish(b => (IStmt)new BlockStmt(b)),
            "return".Appended().Extend(expr.Optional()).Append(";").Finish(e => (IStmt)new ReturnStmt(e)),
            type.Extended().Extend(id).Extend(init).Append(";").Finish((t, n, i) => (IStmt)new LocalVarStmt(t, n, i)),
            expr.Extended().Append(";").Finish(e => (IStmt)new ExprStmt(e)),
            expr.Extended().Append("=").Extend(expr).Append(";").Finish((l, r) => (IStmt)new AssignStmt(l, r)));

        closed.SetProductions(
            simple.AsProduction(),
            "if".Appended().Append("(").Extend(expr).Append(")").Extend(closed).Append("else").Extend(closed)
                .Finish((c, t, e) => (IStmt)new IfStmt(c, t, e)),
            "while".Appended().Append("(").Extend(expr).Append(")").Extend(closed).Finish((c, b) => (IStmt)new WhileStmt(c, b)));
        open.SetProductions(
            "if".Appended().Append("(").Extend(expr).Append(")").Extend(stmt).Finish((c, t) => (IStmt)new IfStmt(c, t, null)),
            "if".Appended().Append("(").Extend(expr).Append(")").Extend(closed).Append("else").Extend(open)
                .Finish((c, t, e) => (IStmt)new IfStmt(c, t, e)),
            "while".Appended().Append("(").Extend(expr).Append(")").Extend(open).Finish((c, b) => (IStmt)new WhileStmt(c, b)));
        stmt.SetProductions(closed.AsProduction(), open.AsProduction());

        var modifier = Nonterminal.Create<string>("Modifier", [.. G.Modifiers.Select(m => m.FinishConstant(m))]);
        var param = Nonterminal.Create("Parameter", type.Extended().Extend(id).Finish((t, n) => new Param(t, n)));
        var memberTail = Nonterminal.Create(
            "MemberTail",
            "(".Appended().Extend(param.SeparatedBy<Param, List<Param>>(Terminal.Literal(","))).Append(")").Extend(block)
                .Finish((p, b) => (MemberTail)((m, t, n) => new MethodDecl(m, t, n, new Seq<Param>(p), b))),
            init.Extended().Append(";").Finish(i => (MemberTail)((m, t, n) => new FieldDecl(m, t, n, i))));
        var member = Nonterminal.Create(
            "Member",
            modifier.Many<string, List<string>>().Extended().Extend(type).Extend(id).Extend(memberTail)
                .Finish((m, t, n, tail) => tail(new Seq<string>(m), t, n)));
        var classDecl = Nonterminal.Create(
            "Class",
            "class".Appended().Extend(id).Extend(Nonterminal.Create("Extends", "extends".Appended().Extend(id).AsProduction()).Optional())
                .Append("{").Extend(member.Many<IMember, List<IMember>>()).Append("}")
                .Finish((n, e, ms) => new ClassDecl(n, e, new Seq<IMember>(ms))));
        var unit = Nonterminal.Create(
            "CompilationUnit",
            classDecl.Many<ClassDecl, List<ClassDecl>>().Finish(cs => new CompilationUnit(new Seq<ClassDecl>(cs))));

        var parser = unit.CaseSensitive(true).Build();
        return parser.IsFailing
            ? throw new InvalidOperationException("Farkle couldn't build the MiniJava grammar: " + parser.Parse(string.Empty).Error)
            : parser;
    }
}
