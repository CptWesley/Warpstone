using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Warpstone;

namespace ClausewitzLsp.Core.Parsing;

/// <summary>
/// Provides logic for parsing.
/// </summary>
[SuppressMessage("Ordering Rules", "SA1202", Justification = "Public fields use the private fields.")]
public static class Parser
{
    private static readonly IParser<string> Recovery = CompiledRegex(@"[^\s{}=]*");
    private static readonly IParser<char> OpeningBracket = Char('{');
    private static readonly IParser<char> ClosingBracket = Char('}');
    private static readonly IParser<char> Period = Char('.');
    private static readonly IParser<string> NaturalNumber = CompiledRegex(@"\d+(?!\w)").WithName("number");

    /// <summary>
    /// Provides a parser that parses mandatory layout.
    /// </summary>
    public static readonly IParser<string> MandatoryLayout = CompiledRegex(@"(#.*|\s+)+").WithName("layout");

    /// <summary>
    /// Provides a parser that parses optional layout.
    /// </summary>
    public static readonly IParser<string> OptionalLayout = CompiledRegex(@"(#.*|\s+)*").WithName("layout");

    /// <summary>
    /// Provides a parser that parses an integer.
    /// </summary>
    public static readonly IParser<ParseIntExpr> Integer
        = CompiledRegex(@"-?\d+(?!\w)")
        .Transform(x => new ParseIntExpr(int.Parse(x, CultureInfo.InvariantCulture))).WithName("integer");

    /// <summary>
    /// Provides a parser that parses a floating point number.
    /// </summary>
    public static readonly IParser<ParseFloatExpr> Float
        = CompiledRegex(@"-?\d*\.\d+(?!\w)")
        .Transform(x => new ParseFloatExpr(float.Parse(x, CultureInfo.InvariantCulture))).WithName("float");

    /// <summary>
    /// Provides a parser that parses a truthy boolean.
    /// </summary>
    public static readonly IParser<ParseBoolExpr> Yes
        = CompiledRegex(@"yes(?!\w)")
        .Transform(x => new ParseBoolExpr(true));

    /// <summary>
    /// Provides a parser that parses a falsy boolean.
    /// </summary>
    public static readonly IParser<ParseBoolExpr> No
        = CompiledRegex(@"no(?!\w)")
        .Transform(x => new ParseBoolExpr(false));

    /// <summary>
    /// Provides a parser that parses a boolean.
    /// </summary>
    public static readonly IParser<ParseBoolExpr> Boolean = Or(Yes, No).WithName("boolean");

    /// <summary>
    /// Provides a parser that parses a string.
    /// </summary>
    public static readonly IParser<ParseStringExpr> String
        = Char('"')
        .Then(CompiledRegex("[^\"]*"))
        .ThenSkip(Char('"'))
        .Transform(x => new ParseStringExpr(x)).WithName("string");

    /// <summary>
    /// Provides a parser that parses an identifiet.
    /// </summary>
    public static readonly IParser<ParseIdentifierExpr> Identifier
        = CompiledRegex(@"[\w\._:\-@]+(?!\w)")
        .Transform(x => new ParseIdentifierExpr(x)).WithName("identifier");

    /// <summary>
    /// Provides a parser that parses a date.
    /// </summary>
    public static readonly IParser<ParseDateExpr> Date
        = NaturalNumber
        .ThenSkip(Period)
        .ThenAdd(NaturalNumber)
        .ThenSkip(Period)
        .ThenAdd(NaturalNumber)
        .Transform((y, m, d) => new ParseDateExpr(int.Parse(y, CultureInfo.InvariantCulture), int.Parse(m, CultureInfo.InvariantCulture), int.Parse(d, CultureInfo.InvariantCulture)))
        .WithName("date");

    /// <summary>
    /// Provides a parser that parses a field.
    /// </summary>
    public static readonly IParser<ParseFieldExpr> Field
        = Try(Lazy(() => Value!))
        .ThenSkip(OptionalLayout)
        .ThenSkip(CompiledRegex("="))
        .ThenSkip(OptionalLayout)
        .ThenAdd(Try(Lazy(() => Value!)))
        .Transform((n, e) => new ParseFieldExpr(n, e));

    /// <summary>
    /// Provides a parser that parses a file body.
    /// </summary>
    public static readonly IParser<ParseObjectExpr> FileBody
        = new TryManyParser<ParseExpr>(OptionalLayout, Lazy(() => Expr!), MandatoryLayout, OptionalLayout.ThenEnd(), Recovery)
        .Transform(x => new ParseObjectExpr(x));

    /// <summary>
    /// Provides a parser that parses a file body.
    /// </summary>
    public static readonly IParser<ParseOption<ParseObjectExpr>> TryFileBody = Try(FileBody);

    /// <summary>
    /// Provides a parser that parses an object.
    /// </summary>
    public static readonly IParser<ParseObjectExpr> Object
        = new TryManyParser<ParseExpr>(OpeningBracket.Then(OptionalLayout), Lazy(() => Expr!), MandatoryLayout, OptionalLayout.ThenSkip(ClosingBracket), Recovery)
        .Transform(x => new ParseObjectExpr(x));

    /// <summary>
    /// Provides a parser that parses a value.
    /// </summary>
    public static readonly IParser<ParseExpr> Value
        = Or<ParseExpr>(Object, String, Boolean, Date, Float, Integer, Identifier);

    /// <summary>
    /// Provides a parser that parses an expression.
    /// </summary>
    public static readonly IParser<ParseExpr> Expr
        = Or<ParseExpr>(Field, Value);

    /// <summary>
    /// Creates a file parser for a given file name.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <returns>The parser.</returns>
    public static IParser<ParseFileExpr> File(string fileName)
        => TryFileBody.Transform(x => new ParseFileExpr(fileName, x));

    /// <summary>
    /// Parses a file.
    /// </summary>
    /// <param name="fileName">The path of the file to parse.</param>
    /// <param name="fileContent">The content of the file to parse.</param>
    /// <returns>The parsed file.</returns>
    public static ParseFileExpr ParseFile(string fileName, string fileContent)
    {
        IParser<ParseFileExpr> parser = File(fileName);
        return parser.Parse(fileContent);
    }

    /// <summary>
    /// Parses a file.
    /// </summary>
    /// <param name="fileName">The path of the file to parse.</param>
    /// <returns>The parsed file.</returns>
    public static ParseFileExpr ParseFile(string fileName)
        => ParseFile(fileName, System.IO.File.ReadAllText(fileName, Encoding.GetEncoding("ISO-8859-1")));

    private static IParser<TOut> Try<TIn, TOut>(IParser<TIn> parser, Func<IParseResult<TIn>, TOut> resultTransformation)
        => new TryParser<TIn, TOut>(parser, Recovery, resultTransformation);

    private static IParser<TOut> Try<TIn, TOut>(IParser<TIn> parser, Func<TIn, TOut> successTransformation, Func<IParseError, TOut> failureTransformation)
        => Try(parser, result =>
        {
            if (result.Success)
            {
                return successTransformation(result.Value!);
            }

            return failureTransformation(result.Error!);
        });

    private static IParser<T> Try<T>(IParser<T> parser, Func<IParseError, T> failureTransformation)
        => Try(parser, value => value, error => failureTransformation(error));

    private static IParser<ParseOption<T>> Try<T>(IParser<T> parser)
        => Try(parser, value => new ParseSome<T>(value) as ParseOption<T>, error => new ParseNone<T>(error) as ParseOption<T>);
}
