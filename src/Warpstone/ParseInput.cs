namespace Warpstone;

/// <summary>
/// Represents information for inputs used by parsers.
/// Basic concrete implementation of the <see cref="IParseInput"/> interface.
/// </summary>
public sealed class ParseInput : IParseInput
{
    private ParseInput(IParseInputSource source, string content)
    {
        Source = source;
        Content = content;
    }

    /// <inheritdoc />
    public string Content { get; }

    /// <inheritdoc />
    public IParseInputSource Source { get; }

    /// <summary>
    /// Creates a new <see cref="IParseInput"/> instance
    /// from the given <paramref name="content"/> string
    /// with with the given <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The source of the input.</param>
    /// <param name="content">The input string.</param>
    /// <returns>A new <see cref="IParseInput"/> instance.</returns>
    public static IParseInput Create(IParseInputSource source, string content)
        => new ParseInput(source, content);

    /// <summary>
    /// Creates a new <see cref="IParseInput"/> instance
    /// from the given <paramref name="input"/> string
    /// with a from-memory source.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>A new <see cref="IParseInput"/> instance.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseInput CreateFromMemory(string input)
        => Create(FromMemorySource.Instance, input);

    /// <summary>
    /// Creates a new <see cref="IParseInput"/> instance
    /// from the given <paramref name="content"/> string
    /// with a file source from the given <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path to the source file.</param>
    /// <param name="content">The input string.</param>
    /// <returns>A new <see cref="IParseInput"/> instance.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseInput CreateFromFile(string path, string content)
        => Create(new FileSource(path), content);

    /// <summary>
    /// Creates a new <see cref="IParseInput"/> instance
    /// from the given <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path to the source file.</param>
    /// <returns>A new <see cref="IParseInput"/> instance.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseInput CreateFromFile(string path)
        => CreateFromFile(path, File.ReadAllText(path));
}
