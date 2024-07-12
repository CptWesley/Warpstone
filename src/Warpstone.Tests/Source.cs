using Microsoft.CodeAnalysis.Text;

namespace Warpstone.Tests;

internal static class Source
{
    public static SourceText Text(this string str) => SourceText.From(str);

    public static SourceSpan Span(this string str) => new(SourceText.From(str));
}
