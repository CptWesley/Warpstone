using System;
using System.Collections.Generic;
using System.Text;

namespace Warpstone;

/// <summary>
/// Options to configure how the parsing should be done.
/// </summary>
public sealed record ParseOptions
{
    /// <summary>
    /// The default <see cref="ParseOptions"/>.
    /// </summary>
    public static readonly ParseOptions Default = new();

    /// <summary>
    /// Determines the execution mode of the parser (default: <see cref="ParserExecutionMode.Auto"/>).
    /// </summary>
    public ParserExecutionMode ExecutionMode { get; init; } = ParserExecutionMode.Auto;

    /// <summary>
    /// Determines if lazy parsers should be eliminated to gain some performance
    /// during parsing (default: <see langword="true"/>).
    /// </summary>
    public bool EnableLazyParserElimination { get; init; } = true;

    /// <summary>
    /// Determines if left recursive grammars should automatically
    /// be transformed in a way to support left recursion
    /// (default: <see langword="true"/>).
    /// </summary>
    public bool EnableAutomaticLeftRecursion { get; init; } = true;

    /// <summary>
    /// Determines if grammars should automatically
    /// be transformed to support memoization
    /// (default: <see langword="true"/>).
    /// </summary>
    public bool EnableAutomaticMemoization { get; init; } = true;
}
