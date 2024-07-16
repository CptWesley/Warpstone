using static Warpstone.Examples.JsonNodeKind;
using Grammr = Warpstone.Grammar<Warpstone.Examples.JsonNodeKind>;

namespace Warpstone.Examples;

public sealed class JsonGrammar : Grammr
{
	public static readonly Grammr ws = regex(@"[ \s\t\r\n]*", WhiteSpaceToken);

	public static readonly Grammr number = regex(@"-?(0|[1-9][0-9]*)(\.[0-9]+)?([eE][+-]?(0|[1-9][0-9]*))?", NumberToken);

	public static readonly Grammr @string = ch('"', QuotationToken) & match(c => c != '"', StringToken) & ch('"', QuotationToken);

	public static readonly Grammr member = ws & @string & ws & ch(':', ColonToken) & Lazy(() => element);

	public static readonly Grammr members = member & (ch(',', CommaToken) & member).Star;

	public static readonly Grammr @object =
	   ch('{', ObjectStartToken) & (members | ws) & ch('}', ObjectEndToken);

	public static readonly Grammr array =
	   ch('[', ArrayStartToken) & (Lazy(() => elements) | ws) & ch(']', ArrayEndToken);

	public static readonly Grammr value =
		@object
		| array
		| @string
		| number
		| str("true", TrueToken)
		| str("false", FalseToken)
		| str("null", NullToken);


	public static readonly Grammr element = ws & value & ws;

	public static readonly Grammr elements = element & (ch(',', CommaToken) & element).Star;

	public static readonly Grammr json = element;
}

public enum JsonNodeKind
{
	None,
	WhiteSpaceToken,
	StringToken,
	TrueToken,
	FalseToken,
	NullToken,
	DigitToken,
	NumberToken,
	CommaToken = ',',
	ColonToken = ':',
	ArrayStartToken = '[',
	ArrayEndToken = ']',
	ObjectStartToken = '{',
	ObjectEndToken = '}',
	DecimalSeparatorToken = '.',
	PlusToken = '+',
	MinusToken = '-',
	ExponentToken = 'e',
	QuotationToken = '"',
}
