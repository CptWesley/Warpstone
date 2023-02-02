using System.Runtime.CompilerServices;

namespace Warpstone.Parsers;

/// <summary>
/// Represents a parser which sequentially applies a sequence of parsers and retains all results.
/// </summary>
/// <typeparam name="T1">The result type of the first parser.</typeparam>
/// <typeparam name="T2">The result type of the second parser.</typeparam>
/// <seealso cref="Parser{T}" />
public sealed class SeqParser<T1, T2>
        : BaseSeqParser<(T1 Value1, T2 Value2)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeqParser{T1, T2}"/> class.
    /// </summary>
    /// <param name="first">The result type of the first parser.</param>
    /// <param name="second">The result type of the second parser.</param>
    public SeqParser(IParser<T1> first, IParser<T2> second)
            : base(first, second)
    {
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected sealed override (T1 Value1, T2 Value2) CreateValue(object?[] values)
            => ((T1)values[0]!, (T2)values[1]!);
}

/// <summary>
/// Represents a parser which sequentially applies a sequence of parsers and retains all results.
/// </summary>
/// <typeparam name="T1">The result type of the first parser.</typeparam>
/// <typeparam name="T2">The result type of the second parser.</typeparam>
/// <typeparam name="T3">The result type of the third parser.</typeparam>
/// <seealso cref="Parser{T}" />
public sealed class SeqParser<T1, T2, T3>
        : BaseSeqParser<(T1 Value1, T2 Value2, T3 Value3)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeqParser{T1, T2, T3}"/> class.
    /// </summary>
    /// <param name="first">The result type of the first parser.</param>
    /// <param name="second">The result type of the second parser.</param>
    /// <param name="third">The result type of the third parser.</param>
    public SeqParser(IParser<T1> first, IParser<T2> second, IParser<T3> third)
            : base(first, second, third)
    {
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected sealed override (T1 Value1, T2 Value2, T3 Value3) CreateValue(object?[] values)
            => ((T1)values[0]!, (T2)values[1]!, (T3)values[2]!);
}

/// <summary>
/// Represents a parser which sequentially applies a sequence of parsers and retains all results.
/// </summary>
/// <typeparam name="T1">The result type of the first parser.</typeparam>
/// <typeparam name="T2">The result type of the second parser.</typeparam>
/// <typeparam name="T3">The result type of the third parser.</typeparam>
/// <typeparam name="T4">The result type of the fourth parser.</typeparam>
/// <seealso cref="Parser{T}" />
public sealed class SeqParser<T1, T2, T3, T4>
        : BaseSeqParser<(T1 Value1, T2 Value2, T3 Value3, T4 Value4)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeqParser{T1, T2, T3, T4}"/> class.
    /// </summary>
    /// <param name="first">The result type of the first parser.</param>
    /// <param name="second">The result type of the second parser.</param>
    /// <param name="third">The result type of the third parser.</param>
    /// <param name="fourth">The result type of the fourth parser.</param>
    public SeqParser(IParser<T1> first, IParser<T2> second, IParser<T3> third, IParser<T4> fourth)
            : base(first, second, third, fourth)
    {
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected sealed override (T1 Value1, T2 Value2, T3 Value3, T4 Value4) CreateValue(object?[] values)
            => ((T1)values[0]!, (T2)values[1]!, (T3)values[2]!, (T4)values[3]!);
}

/// <summary>
/// Represents a parser which sequentially applies a sequence of parsers and retains all results.
/// </summary>
/// <typeparam name="T1">The result type of the first parser.</typeparam>
/// <typeparam name="T2">The result type of the second parser.</typeparam>
/// <typeparam name="T3">The result type of the third parser.</typeparam>
/// <typeparam name="T4">The result type of the fourth parser.</typeparam>
/// <typeparam name="T5">The result type of the fifth parser.</typeparam>
/// <seealso cref="Parser{T}" />
public sealed class SeqParser<T1, T2, T3, T4, T5>
        : BaseSeqParser<(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeqParser{T1, T2, T3, T4, T5}"/> class.
    /// </summary>
    /// <param name="first">The result type of the first parser.</param>
    /// <param name="second">The result type of the second parser.</param>
    /// <param name="third">The result type of the third parser.</param>
    /// <param name="fourth">The result type of the fourth parser.</param>
    /// <param name="fifth">The result type of the fifth parser.</param>
    public SeqParser(IParser<T1> first, IParser<T2> second, IParser<T3> third, IParser<T4> fourth, IParser<T5> fifth)
            : base(first, second, third, fourth, fifth)
    {
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected sealed override (T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5) CreateValue(object?[] values)
            => ((T1)values[0]!, (T2)values[1]!, (T3)values[2]!, (T4)values[3]!, (T5)values[4]!);
}

/// <summary>
/// Represents a parser which sequentially applies a sequence of parsers and retains all results.
/// </summary>
/// <typeparam name="T1">The result type of the first parser.</typeparam>
/// <typeparam name="T2">The result type of the second parser.</typeparam>
/// <typeparam name="T3">The result type of the third parser.</typeparam>
/// <typeparam name="T4">The result type of the fourth parser.</typeparam>
/// <typeparam name="T5">The result type of the fifth parser.</typeparam>
/// <typeparam name="T6">The result type of the sixth parser.</typeparam>
/// <seealso cref="Parser{T}" />
public sealed class SeqParser<T1, T2, T3, T4, T5, T6>
        : BaseSeqParser<(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeqParser{T1, T2, T3, T4, T5, T6}"/> class.
    /// </summary>
    /// <param name="first">The result type of the first parser.</param>
    /// <param name="second">The result type of the second parser.</param>
    /// <param name="third">The result type of the third parser.</param>
    /// <param name="fourth">The result type of the fourth parser.</param>
    /// <param name="fifth">The result type of the fifth parser.</param>
    /// <param name="sixth">The result type of the sixth parser.</param>
    public SeqParser(IParser<T1> first, IParser<T2> second, IParser<T3> third, IParser<T4> fourth, IParser<T5> fifth, IParser<T6> sixth)
            : base(first, second, third, fourth, fifth, sixth)
    {
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected sealed override (T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6) CreateValue(object?[] values)
            => ((T1)values[0]!, (T2)values[1]!, (T3)values[2]!, (T4)values[3]!, (T5)values[4]!, (T6)values[5]!);
}

/// <summary>
/// Represents a parser which sequentially applies a sequence of parsers and retains all results.
/// </summary>
/// <typeparam name="T1">The result type of the first parser.</typeparam>
/// <typeparam name="T2">The result type of the second parser.</typeparam>
/// <typeparam name="T3">The result type of the third parser.</typeparam>
/// <typeparam name="T4">The result type of the fourth parser.</typeparam>
/// <typeparam name="T5">The result type of the fifth parser.</typeparam>
/// <typeparam name="T6">The result type of the sixth parser.</typeparam>
/// <typeparam name="T7">The result type of the seventh parser.</typeparam>
/// <seealso cref="Parser{T}" />
public sealed class SeqParser<T1, T2, T3, T4, T5, T6, T7>
        : BaseSeqParser<(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeqParser{T1, T2, T3, T4, T5, T6, T7}"/> class.
    /// </summary>
    /// <param name="first">The result type of the first parser.</param>
    /// <param name="second">The result type of the second parser.</param>
    /// <param name="third">The result type of the third parser.</param>
    /// <param name="fourth">The result type of the fourth parser.</param>
    /// <param name="fifth">The result type of the fifth parser.</param>
    /// <param name="sixth">The result type of the sixth parser.</param>
    /// <param name="seventh">The result type of the seventh parser.</param>
    public SeqParser(IParser<T1> first, IParser<T2> second, IParser<T3> third, IParser<T4> fourth, IParser<T5> fifth, IParser<T6> sixth, IParser<T7> seventh)
            : base(first, second, third, fourth, fifth, sixth, seventh)
    {
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected sealed override (T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7) CreateValue(object?[] values)
            => ((T1)values[0]!, (T2)values[1]!, (T3)values[2]!, (T4)values[3]!, (T5)values[4]!, (T6)values[5]!, (T7)values[6]!);
}

/// <summary>
/// Represents a parser which sequentially applies a sequence of parsers and retains all results.
/// </summary>
/// <typeparam name="T1">The result type of the first parser.</typeparam>
/// <typeparam name="T2">The result type of the second parser.</typeparam>
/// <typeparam name="T3">The result type of the third parser.</typeparam>
/// <typeparam name="T4">The result type of the fourth parser.</typeparam>
/// <typeparam name="T5">The result type of the fifth parser.</typeparam>
/// <typeparam name="T6">The result type of the sixth parser.</typeparam>
/// <typeparam name="T7">The result type of the seventh parser.</typeparam>
/// <typeparam name="T8">The result type of the eighth parser.</typeparam>
/// <seealso cref="Parser{T}" />
public sealed class SeqParser<T1, T2, T3, T4, T5, T6, T7, T8>
        : BaseSeqParser<(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeqParser{T1, T2, T3, T4, T5, T6, T7, T8}"/> class.
    /// </summary>
    /// <param name="first">The result type of the first parser.</param>
    /// <param name="second">The result type of the second parser.</param>
    /// <param name="third">The result type of the third parser.</param>
    /// <param name="fourth">The result type of the fourth parser.</param>
    /// <param name="fifth">The result type of the fifth parser.</param>
    /// <param name="sixth">The result type of the sixth parser.</param>
    /// <param name="seventh">The result type of the seventh parser.</param>
    /// <param name="eighth">The result type of the eighth parser.</param>
    public SeqParser(IParser<T1> first, IParser<T2> second, IParser<T3> third, IParser<T4> fourth, IParser<T5> fifth, IParser<T6> sixth, IParser<T7> seventh, IParser<T8> eighth)
            : base(first, second, third, fourth, fifth, sixth, seventh, eighth)
    {
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected sealed override (T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8) CreateValue(object?[] values)
            => ((T1)values[0]!, (T2)values[1]!, (T3)values[2]!, (T4)values[3]!, (T5)values[4]!, (T6)values[5]!, (T7)values[6]!, (T8)values[7]!);
}

/// <summary>
/// Represents a parser which sequentially applies a sequence of parsers and retains all results.
/// </summary>
/// <typeparam name="T1">The result type of the first parser.</typeparam>
/// <typeparam name="T2">The result type of the second parser.</typeparam>
/// <typeparam name="T3">The result type of the third parser.</typeparam>
/// <typeparam name="T4">The result type of the fourth parser.</typeparam>
/// <typeparam name="T5">The result type of the fifth parser.</typeparam>
/// <typeparam name="T6">The result type of the sixth parser.</typeparam>
/// <typeparam name="T7">The result type of the seventh parser.</typeparam>
/// <typeparam name="T8">The result type of the eighth parser.</typeparam>
/// <typeparam name="T9">The result type of the nineth parser.</typeparam>
/// <seealso cref="Parser{T}" />
public sealed class SeqParser<T1, T2, T3, T4, T5, T6, T7, T8, T9>
        : BaseSeqParser<(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeqParser{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> class.
    /// </summary>
    /// <param name="first">The result type of the first parser.</param>
    /// <param name="second">The result type of the second parser.</param>
    /// <param name="third">The result type of the third parser.</param>
    /// <param name="fourth">The result type of the fourth parser.</param>
    /// <param name="fifth">The result type of the fifth parser.</param>
    /// <param name="sixth">The result type of the sixth parser.</param>
    /// <param name="seventh">The result type of the seventh parser.</param>
    /// <param name="eighth">The result type of the eighth parser.</param>
    /// <param name="nineth">The result type of the nineth parser.</param>
    public SeqParser(IParser<T1> first, IParser<T2> second, IParser<T3> third, IParser<T4> fourth, IParser<T5> fifth, IParser<T6> sixth, IParser<T7> seventh, IParser<T8> eighth, IParser<T9> nineth)
            : base(first, second, third, fourth, fifth, sixth, seventh, eighth, nineth)
    {
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected sealed override (T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9) CreateValue(object?[] values)
            => ((T1)values[0]!, (T2)values[1]!, (T3)values[2]!, (T4)values[3]!, (T5)values[4]!, (T6)values[5]!, (T7)values[6]!, (T8)values[7]!, (T9)values[8]!);
}

/// <summary>
/// Represents a parser which sequentially applies a sequence of parsers and retains all results.
/// </summary>
/// <typeparam name="T1">The result type of the first parser.</typeparam>
/// <typeparam name="T2">The result type of the second parser.</typeparam>
/// <typeparam name="T3">The result type of the third parser.</typeparam>
/// <typeparam name="T4">The result type of the fourth parser.</typeparam>
/// <typeparam name="T5">The result type of the fifth parser.</typeparam>
/// <typeparam name="T6">The result type of the sixth parser.</typeparam>
/// <typeparam name="T7">The result type of the seventh parser.</typeparam>
/// <typeparam name="T8">The result type of the eighth parser.</typeparam>
/// <typeparam name="T9">The result type of the nineth parser.</typeparam>
/// <typeparam name="T10">The result type of the tenth parser.</typeparam>
/// <seealso cref="Parser{T}" />
public sealed class SeqParser<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
        : BaseSeqParser<(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeqParser{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/> class.
    /// </summary>
    /// <param name="first">The result type of the first parser.</param>
    /// <param name="second">The result type of the second parser.</param>
    /// <param name="third">The result type of the third parser.</param>
    /// <param name="fourth">The result type of the fourth parser.</param>
    /// <param name="fifth">The result type of the fifth parser.</param>
    /// <param name="sixth">The result type of the sixth parser.</param>
    /// <param name="seventh">The result type of the seventh parser.</param>
    /// <param name="eighth">The result type of the eighth parser.</param>
    /// <param name="nineth">The result type of the nineth parser.</param>
    /// <param name="tenth">The result type of the tenth parser.</param>
    public SeqParser(IParser<T1> first, IParser<T2> second, IParser<T3> third, IParser<T4> fourth, IParser<T5> fifth, IParser<T6> sixth, IParser<T7> seventh, IParser<T8> eighth, IParser<T9> nineth, IParser<T10> tenth)
            : base(first, second, third, fourth, fifth, sixth, seventh, eighth, nineth, tenth)
    {
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected sealed override (T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10) CreateValue(object?[] values)
            => ((T1)values[0]!, (T2)values[1]!, (T3)values[2]!, (T4)values[3]!, (T5)values[4]!, (T6)values[5]!, (T7)values[6]!, (T8)values[7]!, (T9)values[8]!, (T10)values[9]!);
}

/// <summary>
/// Represents a parser which sequentially applies a sequence of parsers and retains all results.
/// </summary>
/// <typeparam name="T1">The result type of the first parser.</typeparam>
/// <typeparam name="T2">The result type of the second parser.</typeparam>
/// <typeparam name="T3">The result type of the third parser.</typeparam>
/// <typeparam name="T4">The result type of the fourth parser.</typeparam>
/// <typeparam name="T5">The result type of the fifth parser.</typeparam>
/// <typeparam name="T6">The result type of the sixth parser.</typeparam>
/// <typeparam name="T7">The result type of the seventh parser.</typeparam>
/// <typeparam name="T8">The result type of the eighth parser.</typeparam>
/// <typeparam name="T9">The result type of the nineth parser.</typeparam>
/// <typeparam name="T10">The result type of the tenth parser.</typeparam>
/// <typeparam name="T11">The result type of the eleventh parser.</typeparam>
/// <seealso cref="Parser{T}" />
public sealed class SeqParser<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
        : BaseSeqParser<(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeqParser{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/> class.
    /// </summary>
    /// <param name="first">The result type of the first parser.</param>
    /// <param name="second">The result type of the second parser.</param>
    /// <param name="third">The result type of the third parser.</param>
    /// <param name="fourth">The result type of the fourth parser.</param>
    /// <param name="fifth">The result type of the fifth parser.</param>
    /// <param name="sixth">The result type of the sixth parser.</param>
    /// <param name="seventh">The result type of the seventh parser.</param>
    /// <param name="eighth">The result type of the eighth parser.</param>
    /// <param name="nineth">The result type of the nineth parser.</param>
    /// <param name="tenth">The result type of the tenth parser.</param>
    /// <param name="eleventh">The result type of the eleventh parser.</param>
    public SeqParser(IParser<T1> first, IParser<T2> second, IParser<T3> third, IParser<T4> fourth, IParser<T5> fifth, IParser<T6> sixth, IParser<T7> seventh, IParser<T8> eighth, IParser<T9> nineth, IParser<T10> tenth, IParser<T11> eleventh)
            : base(first, second, third, fourth, fifth, sixth, seventh, eighth, nineth, tenth, eleventh)
    {
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected sealed override (T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11) CreateValue(object?[] values)
            => ((T1)values[0]!, (T2)values[1]!, (T3)values[2]!, (T4)values[3]!, (T5)values[4]!, (T6)values[5]!, (T7)values[6]!, (T8)values[7]!, (T9)values[8]!, (T10)values[9]!, (T11)values[10]!);
}

/// <summary>
/// Represents a parser which sequentially applies a sequence of parsers and retains all results.
/// </summary>
/// <typeparam name="T1">The result type of the first parser.</typeparam>
/// <typeparam name="T2">The result type of the second parser.</typeparam>
/// <typeparam name="T3">The result type of the third parser.</typeparam>
/// <typeparam name="T4">The result type of the fourth parser.</typeparam>
/// <typeparam name="T5">The result type of the fifth parser.</typeparam>
/// <typeparam name="T6">The result type of the sixth parser.</typeparam>
/// <typeparam name="T7">The result type of the seventh parser.</typeparam>
/// <typeparam name="T8">The result type of the eighth parser.</typeparam>
/// <typeparam name="T9">The result type of the nineth parser.</typeparam>
/// <typeparam name="T10">The result type of the tenth parser.</typeparam>
/// <typeparam name="T11">The result type of the eleventh parser.</typeparam>
/// <typeparam name="T12">The result type of the twelfth parser.</typeparam>
/// <seealso cref="Parser{T}" />
public sealed class SeqParser<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
        : BaseSeqParser<(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11, T12 Value12)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeqParser{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/> class.
    /// </summary>
    /// <param name="first">The result type of the first parser.</param>
    /// <param name="second">The result type of the second parser.</param>
    /// <param name="third">The result type of the third parser.</param>
    /// <param name="fourth">The result type of the fourth parser.</param>
    /// <param name="fifth">The result type of the fifth parser.</param>
    /// <param name="sixth">The result type of the sixth parser.</param>
    /// <param name="seventh">The result type of the seventh parser.</param>
    /// <param name="eighth">The result type of the eighth parser.</param>
    /// <param name="nineth">The result type of the nineth parser.</param>
    /// <param name="tenth">The result type of the tenth parser.</param>
    /// <param name="eleventh">The result type of the eleventh parser.</param>
    /// <param name="twelfth">The result type of the twelfth parser.</param>
    public SeqParser(IParser<T1> first, IParser<T2> second, IParser<T3> third, IParser<T4> fourth, IParser<T5> fifth, IParser<T6> sixth, IParser<T7> seventh, IParser<T8> eighth, IParser<T9> nineth, IParser<T10> tenth, IParser<T11> eleventh, IParser<T12> twelfth)
            : base(first, second, third, fourth, fifth, sixth, seventh, eighth, nineth, tenth, eleventh, twelfth)
    {
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected sealed override (T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11, T12 Value12) CreateValue(object?[] values)
            => ((T1)values[0]!, (T2)values[1]!, (T3)values[2]!, (T4)values[3]!, (T5)values[4]!, (T6)values[5]!, (T7)values[6]!, (T8)values[7]!, (T9)values[8]!, (T10)values[9]!, (T11)values[10]!, (T12)values[11]!);
}

/// <summary>
/// Represents a parser which sequentially applies a sequence of parsers and retains all results.
/// </summary>
/// <typeparam name="T1">The result type of the first parser.</typeparam>
/// <typeparam name="T2">The result type of the second parser.</typeparam>
/// <typeparam name="T3">The result type of the third parser.</typeparam>
/// <typeparam name="T4">The result type of the fourth parser.</typeparam>
/// <typeparam name="T5">The result type of the fifth parser.</typeparam>
/// <typeparam name="T6">The result type of the sixth parser.</typeparam>
/// <typeparam name="T7">The result type of the seventh parser.</typeparam>
/// <typeparam name="T8">The result type of the eighth parser.</typeparam>
/// <typeparam name="T9">The result type of the nineth parser.</typeparam>
/// <typeparam name="T10">The result type of the tenth parser.</typeparam>
/// <typeparam name="T11">The result type of the eleventh parser.</typeparam>
/// <typeparam name="T12">The result type of the twelfth parser.</typeparam>
/// <typeparam name="T13">The result type of the thirteenth parser.</typeparam>
/// <seealso cref="Parser{T}" />
public sealed class SeqParser<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
        : BaseSeqParser<(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11, T12 Value12, T13 Value13)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeqParser{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/> class.
    /// </summary>
    /// <param name="first">The result type of the first parser.</param>
    /// <param name="second">The result type of the second parser.</param>
    /// <param name="third">The result type of the third parser.</param>
    /// <param name="fourth">The result type of the fourth parser.</param>
    /// <param name="fifth">The result type of the fifth parser.</param>
    /// <param name="sixth">The result type of the sixth parser.</param>
    /// <param name="seventh">The result type of the seventh parser.</param>
    /// <param name="eighth">The result type of the eighth parser.</param>
    /// <param name="nineth">The result type of the nineth parser.</param>
    /// <param name="tenth">The result type of the tenth parser.</param>
    /// <param name="eleventh">The result type of the eleventh parser.</param>
    /// <param name="twelfth">The result type of the twelfth parser.</param>
    /// <param name="thirteenth">The result type of the thirteenth parser.</param>
    public SeqParser(IParser<T1> first, IParser<T2> second, IParser<T3> third, IParser<T4> fourth, IParser<T5> fifth, IParser<T6> sixth, IParser<T7> seventh, IParser<T8> eighth, IParser<T9> nineth, IParser<T10> tenth, IParser<T11> eleventh, IParser<T12> twelfth, IParser<T13> thirteenth)
            : base(first, second, third, fourth, fifth, sixth, seventh, eighth, nineth, tenth, eleventh, twelfth, thirteenth)
    {
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected sealed override (T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11, T12 Value12, T13 Value13) CreateValue(object?[] values)
            => ((T1)values[0]!, (T2)values[1]!, (T3)values[2]!, (T4)values[3]!, (T5)values[4]!, (T6)values[5]!, (T7)values[6]!, (T8)values[7]!, (T9)values[8]!, (T10)values[9]!, (T11)values[10]!, (T12)values[11]!, (T13)values[12]!);
}

/// <summary>
/// Represents a parser which sequentially applies a sequence of parsers and retains all results.
/// </summary>
/// <typeparam name="T1">The result type of the first parser.</typeparam>
/// <typeparam name="T2">The result type of the second parser.</typeparam>
/// <typeparam name="T3">The result type of the third parser.</typeparam>
/// <typeparam name="T4">The result type of the fourth parser.</typeparam>
/// <typeparam name="T5">The result type of the fifth parser.</typeparam>
/// <typeparam name="T6">The result type of the sixth parser.</typeparam>
/// <typeparam name="T7">The result type of the seventh parser.</typeparam>
/// <typeparam name="T8">The result type of the eighth parser.</typeparam>
/// <typeparam name="T9">The result type of the nineth parser.</typeparam>
/// <typeparam name="T10">The result type of the tenth parser.</typeparam>
/// <typeparam name="T11">The result type of the eleventh parser.</typeparam>
/// <typeparam name="T12">The result type of the twelfth parser.</typeparam>
/// <typeparam name="T13">The result type of the thirteenth parser.</typeparam>
/// <typeparam name="T14">The result type of the fourteenth parser.</typeparam>
/// <seealso cref="Parser{T}" />
public sealed class SeqParser<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
        : BaseSeqParser<(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11, T12 Value12, T13 Value13, T14 Value14)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeqParser{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/> class.
    /// </summary>
    /// <param name="first">The result type of the first parser.</param>
    /// <param name="second">The result type of the second parser.</param>
    /// <param name="third">The result type of the third parser.</param>
    /// <param name="fourth">The result type of the fourth parser.</param>
    /// <param name="fifth">The result type of the fifth parser.</param>
    /// <param name="sixth">The result type of the sixth parser.</param>
    /// <param name="seventh">The result type of the seventh parser.</param>
    /// <param name="eighth">The result type of the eighth parser.</param>
    /// <param name="nineth">The result type of the nineth parser.</param>
    /// <param name="tenth">The result type of the tenth parser.</param>
    /// <param name="eleventh">The result type of the eleventh parser.</param>
    /// <param name="twelfth">The result type of the twelfth parser.</param>
    /// <param name="thirteenth">The result type of the thirteenth parser.</param>
    /// <param name="fourteenth">The result type of the fourteenth parser.</param>
    public SeqParser(IParser<T1> first, IParser<T2> second, IParser<T3> third, IParser<T4> fourth, IParser<T5> fifth, IParser<T6> sixth, IParser<T7> seventh, IParser<T8> eighth, IParser<T9> nineth, IParser<T10> tenth, IParser<T11> eleventh, IParser<T12> twelfth, IParser<T13> thirteenth, IParser<T14> fourteenth)
            : base(first, second, third, fourth, fifth, sixth, seventh, eighth, nineth, tenth, eleventh, twelfth, thirteenth, fourteenth)
    {
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected sealed override (T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11, T12 Value12, T13 Value13, T14 Value14) CreateValue(object?[] values)
            => ((T1)values[0]!, (T2)values[1]!, (T3)values[2]!, (T4)values[3]!, (T5)values[4]!, (T6)values[5]!, (T7)values[6]!, (T8)values[7]!, (T9)values[8]!, (T10)values[9]!, (T11)values[10]!, (T12)values[11]!, (T13)values[12]!, (T14)values[13]!);
}

/// <summary>
/// Represents a parser which sequentially applies a sequence of parsers and retains all results.
/// </summary>
/// <typeparam name="T1">The result type of the first parser.</typeparam>
/// <typeparam name="T2">The result type of the second parser.</typeparam>
/// <typeparam name="T3">The result type of the third parser.</typeparam>
/// <typeparam name="T4">The result type of the fourth parser.</typeparam>
/// <typeparam name="T5">The result type of the fifth parser.</typeparam>
/// <typeparam name="T6">The result type of the sixth parser.</typeparam>
/// <typeparam name="T7">The result type of the seventh parser.</typeparam>
/// <typeparam name="T8">The result type of the eighth parser.</typeparam>
/// <typeparam name="T9">The result type of the nineth parser.</typeparam>
/// <typeparam name="T10">The result type of the tenth parser.</typeparam>
/// <typeparam name="T11">The result type of the eleventh parser.</typeparam>
/// <typeparam name="T12">The result type of the twelfth parser.</typeparam>
/// <typeparam name="T13">The result type of the thirteenth parser.</typeparam>
/// <typeparam name="T14">The result type of the fourteenth parser.</typeparam>
/// <typeparam name="T15">The result type of the fifteenth parser.</typeparam>
/// <seealso cref="Parser{T}" />
public sealed class SeqParser<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
        : BaseSeqParser<(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11, T12 Value12, T13 Value13, T14 Value14, T15 Value15)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeqParser{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/> class.
    /// </summary>
    /// <param name="first">The result type of the first parser.</param>
    /// <param name="second">The result type of the second parser.</param>
    /// <param name="third">The result type of the third parser.</param>
    /// <param name="fourth">The result type of the fourth parser.</param>
    /// <param name="fifth">The result type of the fifth parser.</param>
    /// <param name="sixth">The result type of the sixth parser.</param>
    /// <param name="seventh">The result type of the seventh parser.</param>
    /// <param name="eighth">The result type of the eighth parser.</param>
    /// <param name="nineth">The result type of the nineth parser.</param>
    /// <param name="tenth">The result type of the tenth parser.</param>
    /// <param name="eleventh">The result type of the eleventh parser.</param>
    /// <param name="twelfth">The result type of the twelfth parser.</param>
    /// <param name="thirteenth">The result type of the thirteenth parser.</param>
    /// <param name="fourteenth">The result type of the fourteenth parser.</param>
    /// <param name="fifteenth">The result type of the fifteenth parser.</param>
    public SeqParser(IParser<T1> first, IParser<T2> second, IParser<T3> third, IParser<T4> fourth, IParser<T5> fifth, IParser<T6> sixth, IParser<T7> seventh, IParser<T8> eighth, IParser<T9> nineth, IParser<T10> tenth, IParser<T11> eleventh, IParser<T12> twelfth, IParser<T13> thirteenth, IParser<T14> fourteenth, IParser<T15> fifteenth)
            : base(first, second, third, fourth, fifth, sixth, seventh, eighth, nineth, tenth, eleventh, twelfth, thirteenth, fourteenth, fifteenth)
    {
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected sealed override (T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11, T12 Value12, T13 Value13, T14 Value14, T15 Value15) CreateValue(object?[] values)
            => ((T1)values[0]!, (T2)values[1]!, (T3)values[2]!, (T4)values[3]!, (T5)values[4]!, (T6)values[5]!, (T7)values[6]!, (T8)values[7]!, (T9)values[8]!, (T10)values[9]!, (T11)values[10]!, (T12)values[11]!, (T13)values[12]!, (T14)values[13]!, (T15)values[14]!);
}

/// <summary>
/// Represents a parser which sequentially applies a sequence of parsers and retains all results.
/// </summary>
/// <typeparam name="T1">The result type of the first parser.</typeparam>
/// <typeparam name="T2">The result type of the second parser.</typeparam>
/// <typeparam name="T3">The result type of the third parser.</typeparam>
/// <typeparam name="T4">The result type of the fourth parser.</typeparam>
/// <typeparam name="T5">The result type of the fifth parser.</typeparam>
/// <typeparam name="T6">The result type of the sixth parser.</typeparam>
/// <typeparam name="T7">The result type of the seventh parser.</typeparam>
/// <typeparam name="T8">The result type of the eighth parser.</typeparam>
/// <typeparam name="T9">The result type of the nineth parser.</typeparam>
/// <typeparam name="T10">The result type of the tenth parser.</typeparam>
/// <typeparam name="T11">The result type of the eleventh parser.</typeparam>
/// <typeparam name="T12">The result type of the twelfth parser.</typeparam>
/// <typeparam name="T13">The result type of the thirteenth parser.</typeparam>
/// <typeparam name="T14">The result type of the fourteenth parser.</typeparam>
/// <typeparam name="T15">The result type of the fifteenth parser.</typeparam>
/// <typeparam name="T16">The result type of the sixteenth parser.</typeparam>
/// <seealso cref="Parser{T}" />
public sealed class SeqParser<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
        : BaseSeqParser<(T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11, T12 Value12, T13 Value13, T14 Value14, T15 Value15, T16 Value16)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeqParser{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/> class.
    /// </summary>
    /// <param name="first">The result type of the first parser.</param>
    /// <param name="second">The result type of the second parser.</param>
    /// <param name="third">The result type of the third parser.</param>
    /// <param name="fourth">The result type of the fourth parser.</param>
    /// <param name="fifth">The result type of the fifth parser.</param>
    /// <param name="sixth">The result type of the sixth parser.</param>
    /// <param name="seventh">The result type of the seventh parser.</param>
    /// <param name="eighth">The result type of the eighth parser.</param>
    /// <param name="nineth">The result type of the nineth parser.</param>
    /// <param name="tenth">The result type of the tenth parser.</param>
    /// <param name="eleventh">The result type of the eleventh parser.</param>
    /// <param name="twelfth">The result type of the twelfth parser.</param>
    /// <param name="thirteenth">The result type of the thirteenth parser.</param>
    /// <param name="fourteenth">The result type of the fourteenth parser.</param>
    /// <param name="fifteenth">The result type of the fifteenth parser.</param>
    /// <param name="sixteenth">The result type of the sixteenth parser.</param>
    public SeqParser(IParser<T1> first, IParser<T2> second, IParser<T3> third, IParser<T4> fourth, IParser<T5> fifth, IParser<T6> sixth, IParser<T7> seventh, IParser<T8> eighth, IParser<T9> nineth, IParser<T10> tenth, IParser<T11> eleventh, IParser<T12> twelfth, IParser<T13> thirteenth, IParser<T14> fourteenth, IParser<T15> fifteenth, IParser<T16> sixteenth)
            : base(first, second, third, fourth, fifth, sixth, seventh, eighth, nineth, tenth, eleventh, twelfth, thirteenth, fourteenth, fifteenth, sixteenth)
    {
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected sealed override (T1 Value1, T2 Value2, T3 Value3, T4 Value4, T5 Value5, T6 Value6, T7 Value7, T8 Value8, T9 Value9, T10 Value10, T11 Value11, T12 Value12, T13 Value13, T14 Value14, T15 Value15, T16 Value16) CreateValue(object?[] values)
            => ((T1)values[0]!, (T2)values[1]!, (T3)values[2]!, (T4)values[3]!, (T5)values[4]!, (T6)values[5]!, (T7)values[6]!, (T8)values[7]!, (T9)values[8]!, (T10)values[9]!, (T11)values[10]!, (T12)values[11]!, (T13)values[12]!, (T14)values[13]!, (T15)values[14]!, (T16)values[15]!);
}