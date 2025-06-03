using System;
using System.Collections.Generic;
using System.Text;

namespace Warpstone;

/// <summary>
/// Provides helper methods to create <see cref="IParseContext{T}"/> instances.
/// </summary>
public static class ParseContext
{
    private static readonly MethodInfo createMethodInfo
    = typeof(ParseContext)
        .GetRuntimeMethods()
        .First(m
            => m.Name == nameof(Create)
            && m.IsGenericMethodDefinition
            && m.GetParameters().Length == 3
            && m.GetParameters()[0].ParameterType == typeof(IParseInput)
            && m.GetParameters()[2].ParameterType == typeof(ParseOptions));

    private static IParseContext<T> CreateAutomaticContext<T>(IParseInput input, IParser<T> parser)
    {
        // TODO: determine if recursive is safe and use recursive.
        return new IterativeParseContext<T>(input, parser);
    }

    /// <summary>
    /// Creates a new <see cref="IParseContext{T}"/> from the given <paramref name="input"/>
    /// and the initial <paramref name="parser"/>.
    /// </summary>
    /// <param name="input">The input to be parsed.</param>
    /// <param name="parser">The parser that starts the parsing.</param>
    /// <param name="options">The options to use for parsing.</param>
    /// <typeparam name="T">The output type of the initial parser.</typeparam>
    /// <returns>The newly created <see cref="IParseContext{T}"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParseContext<T> Create<T>(IParseInput input, IParser<T> parser, ParseOptions options)
        => options.ExecutionMode switch
        {
            ParserExecutionMode.Iterative => new IterativeParseContext<T>(input, parser),
            ParserExecutionMode.Recursive => new RecursiveParseContext<T>(input, parser),
            _ => CreateAutomaticContext(input, parser),
        };

    /// <summary>
    /// Creates a new <see cref="IParseContext{T}"/> from the given <paramref name="input"/>
    /// and the initial <paramref name="parser"/>.
    /// </summary>
    /// <param name="input">The input to be parsed.</param>
    /// <param name="parser">The parser that starts the parsing.</param>
    /// <param name="options">The options to use for parsing.</param>
    /// <typeparam name="T">The output type of the initial parser.</typeparam>
    /// <returns>The newly created <see cref="IParseContext{T}"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParseContext<T> Create<T>(string input, IParser<T> parser, ParseOptions options)
        => Create(ParseInput.CreateFromMemory(input), parser, options);

    /// <summary>
    /// Creates a new <see cref="IParseContext{T}"/> from the given <paramref name="input"/>
    /// and the initial <paramref name="parser"/>.
    /// </summary>
    /// <param name="input">The input to be parsed.</param>
    /// <param name="parser">The parser that starts the parsing.</param>
    /// <param name="options">The options to use for parsing.</param>
    /// <returns>The newly created <see cref="IParseContext{T}"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParseContext Create(IParseInput input, IParser parser, ParseOptions options)
    {
        var expectedInterface = typeof(IParser<>).MakeGenericType(parser.ResultType);
        if (!parser.GetType().GetInterfaces().Contains(expectedInterface))
        {
            throw new ArgumentException($"Parser does not implement 'IParser<{parser.ResultType.FullName}>'.", nameof(parser));
        }

        var method = createMethodInfo.MakeGenericMethod(parser.ResultType);
        return (IParseContext)method.Invoke(null, [input, parser, options])!;
    }

    /// <summary>
    /// Creates a new <see cref="IParseContext{T}"/> from the given <paramref name="input"/>
    /// and the initial <paramref name="parser"/>.
    /// </summary>
    /// <param name="input">The input to be parsed.</param>
    /// <param name="parser">The parser that starts the parsing.</param>
    /// <param name="options">The options to use for parsing.</param>
    /// <returns>The newly created <see cref="IParseContext{T}"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParseContext Create(string input, IParser parser, ParseOptions options)
        => Create(ParseInput.CreateFromMemory(input), parser, options);

    /// <summary>
    /// Creates a new <see cref="IParseContext{T}"/> from the given <paramref name="input"/>
    /// and the initial <paramref name="parser"/>.
    /// </summary>
    /// <param name="input">The input to be parsed.</param>
    /// <param name="parser">The parser that starts the parsing.</param>
    /// <typeparam name="T">The output type of the initial parser.</typeparam>
    /// <returns>The newly created <see cref="IParseContext{T}"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParseContext<T> Create<T>(IParseInput input, IParser<T> parser)
        => Create(input, parser, ParseOptions.Default);

    /// <summary>
    /// Creates a new <see cref="IParseContext{T}"/> from the given <paramref name="input"/>
    /// and the initial <paramref name="parser"/>.
    /// </summary>
    /// <param name="input">The input to be parsed.</param>
    /// <param name="parser">The parser that starts the parsing.</param>
    /// <typeparam name="T">The output type of the initial parser.</typeparam>
    /// <returns>The newly created <see cref="IParseContext{T}"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParseContext<T> Create<T>(string input, IParser<T> parser)
        => Create(input, parser, ParseOptions.Default);

    /// <summary>
    /// Creates a new <see cref="IParseContext{T}"/> from the given <paramref name="input"/>
    /// and the initial <paramref name="parser"/>.
    /// </summary>
    /// <param name="input">The input to be parsed.</param>
    /// <param name="parser">The parser that starts the parsing.</param>
    /// <returns>The newly created <see cref="IParseContext{T}"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParseContext Create(IParseInput input, IParser parser)
        => Create(input, parser, ParseOptions.Default);

    /// <summary>
    /// Creates a new <see cref="IParseContext{T}"/> from the given <paramref name="input"/>
    /// and the initial <paramref name="parser"/>.
    /// </summary>
    /// <param name="input">The input to be parsed.</param>
    /// <param name="parser">The parser that starts the parsing.</param>
    /// <returns>The newly created <see cref="IParseContext{T}"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParseContext Create(string input, IParser parser)
        => Create(input, parser, ParseOptions.Default);
}
