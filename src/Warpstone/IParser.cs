using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Warpstone;

/// <summary>
/// Interface for all parser implementations.
/// </summary>
public interface IParser
{
    /// <summary>
    /// Gets the result type of the parser.
    /// </summary>
    public Type ResultType { get; }

    /// <summary>
    /// Applies the parser recursively.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    /// <param name="position">The position to apply it to.</param>
    /// <returns>The found result.</returns>
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position);

    /// <summary>
    /// Applies the parser iteratively.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    /// <param name="position">The position to apply it to.</param>
    public void Apply(IIterativeParseContext context, int position);
}

/// <summary>
/// Interface for all typed parser implementations.
/// </summary>
/// <typeparam name="T">The result type being parsed.</typeparam>
#pragma warning disable S2326 // Unused type parameters should be removed
public interface IParser<out T> : IParser;
#pragma warning restore S2326 // Unused type parameters should be removed
