using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Warpstone.Internal;

/// <summary>
/// Provides a covariant wrapper for dictionary access.
/// </summary>
/// <typeparam name="TKey">The exposed key type.</typeparam>
/// <typeparam name="TKeyCo">The internal key type.</typeparam>
/// <typeparam name="TValue">The exposed value type.</typeparam>
/// <typeparam name="TValueCo">The internal value type.</typeparam>
internal class ShallowReadOnlyDictionary<TKey, TKeyCo, TValue, TValueCo> : IReadOnlyDictionary<TKey, TValue>
        where TKeyCo : TKey
        where TValueCo : TValue
{
    private readonly IReadOnlyDictionary<TKeyCo, TValueCo> inner;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShallowReadOnlyDictionary{TKey, TKeyCo, TValue, TValueCo}"/> class.
    /// </summary>
    /// <param name="inner">The wrapped dictionary.</param>
    public ShallowReadOnlyDictionary(IReadOnlyDictionary<TKeyCo, TValueCo> inner)
        => this.inner = inner;

    /// <inheritdoc/>
    public IEnumerable<TKey> Keys => inner.Keys.Select(x => (TKey)x);

    /// <inheritdoc/>
    public IEnumerable<TValue> Values => inner.Values.Select(x => (TValue)x);

    /// <inheritdoc/>
    public int Count => inner.Count;

    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get
        {
            if (key is TKeyCo innerKey)
            {
                return inner[innerKey];
            }

            throw new KeyNotFoundException();
        }
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        if (key is TKeyCo innerKey)
        {
            return inner.ContainsKey(innerKey);
        }

        return false;
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        => inner.Select(x => new KeyValuePair<TKey, TValue>(x.Key, x.Value)).GetEnumerator();

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key is TKeyCo innerKey)
        {
            bool innerResult = inner.TryGetValue(innerKey, out TValueCo innerValue);
            value = innerValue;
            return innerResult;
        }

        value = default!;
        return false;
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
