namespace Legacy.Warpstone1
{
    /// <summary>
    /// Option class for parsing optional syntax.
    /// </summary>
    /// <typeparam name="T">Possible result value.</typeparam>
    public interface IOption<T>
    {
        /// <summary>
        /// Gets a value indicating whether this instance has a value.
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        T Value { get; }
    }

    /// <summary>
    /// Option class for existing values.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the option.</typeparam>
    /// <seealso cref="IOption{T}" />
    public class Some<T> : IOption<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Some{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Some(T value)
            => Value = value;

        /// <inheritdoc/>
        public bool HasValue => true;

        /// <inheritdoc/>
        public T Value { get; }

        /// <summary>
        /// Deconstructs this instance.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Deconstruct(out T value)
            => value = Value;
    }

    /// <summary>
    /// Option class for non existing values.
    /// </summary>
    /// <typeparam name="T">The type of value contained in the option.</typeparam>
    /// <seealso cref="IOption{T}" />
    public class None<T> : IOption<T>
    {
        /// <inheritdoc/>
        public bool HasValue => false;

        /// <inheritdoc/>
        public T Value => default!;

        /// <summary>
        /// Deconstructs this instance.
        /// </summary>
        public void Deconstruct()
        {
        }
    }
}
