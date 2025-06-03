namespace Legacy.Warpstone2.Internal;

/// <summary>
/// Provides guard methods.
/// </summary>
internal static class Guards
{
    /// <summary>
    /// Validates that the given <paramref name="value"/> is not <c>null</c>.
    /// </summary>
    /// <typeparam name="T">The type of <paramref name="value"/>.</typeparam>
    /// <param name="value">The value to validate.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The original <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <c>null</c>.</exception>
    public static T MustNotBeNull<T>(
        [NotNull] this T? value,
        [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }

        return value;
    }

    /// <summary>
    /// Validates that the given <paramref name="value"/> is greater than <paramref name="boundary"/>.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="boundary">The boundary value.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The original <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is less than or equal to <paramref name="boundary"/>.</exception>
    public static int MustBeGreaterThan(
        [NotNull] this int value,
        int boundary,
        [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (value <= boundary)
        {
            throw new ArgumentException($"Value of '{parameterName}' ({value}) must be greater than {boundary}.", parameterName);
        }

        return value;
    }

    /// <summary>
    /// Validates that the given <paramref name="value"/> is greater than <paramref name="boundary"/>.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="boundary">The boundary value.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The original <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is less than or equal to <paramref name="boundary"/>.</exception>
    public static ulong MustBeGreaterThan(
        [NotNull] this ulong value,
        ulong boundary,
        [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (value <= boundary)
        {
            throw new ArgumentException($"Value of '{parameterName}' ({value}) must be greater than {boundary}.", parameterName);
        }

        return value;
    }

    /// <summary>
    /// Validates that the given <paramref name="value"/> is greater than <paramref name="boundary"/>.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="boundary">The boundary value.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The original <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is less than or equal to <paramref name="boundary"/>.</exception>
    public static int MustBeGreaterThanOrEqualTo(
        [NotNull] this int value,
        int boundary,
        [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (value < boundary)
        {
            throw new ArgumentException($"Value of '{parameterName}' ({value}) must be greater than or equal to {boundary}.", parameterName);
        }

        return value;
    }

    /// <summary>
    /// Validates that the given <paramref name="value"/> is greater than <paramref name="boundary"/>.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="boundary">The boundary value.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The original <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is less than or equal to <paramref name="boundary"/>.</exception>
    public static ulong MustBeGreaterThanOrEqualTo(
        [NotNull] this ulong value,
        ulong boundary,
        [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (value < boundary)
        {
            throw new ArgumentException($"Value of '{parameterName}' ({value}) must be greater than or equal to {boundary}.", parameterName);
        }

        return value;
    }
}
