namespace Warpstone.IterativeExecution;

/// <summary>
/// Provides helper methods for dealing with our iterative executor.
/// </summary>
public static class Iterative
{
    /// <summary>
    /// Creates a new iterative step which represents a return statement.
    /// </summary>
    /// <param name="value">The value returned by the return statement.</param>
    /// <returns>The newly created iterative step.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IIterativeStep Done(object? value)
        => new IterativeDone
        {
            Value = value,
        };

    /// <summary>
    /// Creates a new iterative step which represents a return statement
    /// which returns the result of a (recursive) function call.
    /// </summary>
    /// <param name="first">The function that is called and returned.</param>
    /// <returns>The newly created iterative step.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IIterativeStep Done(Func<IIterativeStep> first)
        => More(first, Done);

    /// <summary>
    /// Creates a new iterative step which represents a return statement.
    /// </summary>
    /// <returns>The newly created iterative step.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IIterativeStep Done()
        => Done(null as object);

    /// <summary>
    /// Creates a new iterative step which represents calling
    /// first calling a function and then using its result.
    /// </summary>
    /// <param name="first">The function that is first called.</param>
    /// <param name="more">The function that handles the result
    /// of the <paramref name="first"/> call.</param>
    /// <returns>The newly created iterative step.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IIterativeStep More(Func<IIterativeStep> first, Func<object?, IIterativeStep> more)
        => new IterativeMoreAdHoc
        {
            First = first,
            More = more,
        };
}
