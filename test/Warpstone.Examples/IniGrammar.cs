using static Warpstone.Examples.IniTokenKind;
using Grammr = Warpstone.Grammar<Warpstone.Examples.IniTokenKind>;

namespace Warpstone.Examples;

public sealed class IniGrammar : Grammr
{
	public static readonly Grammr eol = eof | str("\r\n", EoLToken) | ch('\n', EoLToken);

	public static readonly Grammr space = line(@"\s*", WhitespaceToken);

	public static readonly Grammr header = space
	   & ch('[', HeaderStartToken)
	   & line(@"[^]]+", HeaderToken)
	   & ch(']', HeaderEndToken)
	   & space
	   & eol;

	public static readonly Grammr comment =
		space
		& (ch('#', CommentDelimiterToken) | ch(';', CommentDelimiterToken))
		& line(".*", CommentToken);

	public static readonly Grammr key = line(@"[^\s:=]+", KeyToken);

	public static readonly Grammr assign = ch('=', EqualsToken) | ch(':', ColonToken);

	public static readonly Grammr value = line(@"[^\s#;]+", ValueToken);

	public static readonly Grammr kvp = space
	   & key
	   & space
	   & assign
	   & space
	   & value
	   & space
	   & comment.Option;

	public static readonly Grammr single_line = (kvp | comment | space) & eol;

	public static readonly Grammr section = header & single_line.Star;

	public static readonly Grammr file = single_line.Star & section.Star;
}

public enum IniTokenKind
{
	None = 0,
	ValueToken,
	KeyToken,
	WhitespaceToken,
	CommentDelimiterToken,
	CommentToken,
	EoLToken,
	HeaderToken,
	HeaderStartToken = '[',
	HeaderEndToken = ']',
	EqualsToken = '=',
	ColonToken = ':',
}
