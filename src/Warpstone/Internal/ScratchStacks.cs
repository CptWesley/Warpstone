using System;
using System.Threading;

namespace Warpstone.Internal
{
    /// <summary>
    /// The buffers that <see cref="Parsers.Many{TSource}(IParser{TSource})"/> and its relatives collect elements in during one parse,
    /// one stack per element type. A repetition pushes its elements and pops them into an array of the exact size when it ends,
    /// so nested repetitions of the same type share a stack. That costs one array per repetition, instead of a list and the arrays it outgrew.
    /// </summary>
    internal sealed class ScratchStacks
    {
        private static int typeCount;

        private object?[] stacks = new object?[4];

        /// <summary>
        /// Implemented by the parse contexts that provide scratch stacks.
        /// </summary>
        public interface IOwner
        {
            /// <summary>
            /// Gets the scratch stacks of the parse.
            /// </summary>
            public ScratchStacks Scratch { get; }
        }

        /// <summary>
        /// Gets the stack for elements of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <returns>The stack.</returns>
        public Stack<T> Get<T>()
        {
            var slot = Slot<T>.Index;
            if (slot >= stacks.Length)
            {
                Array.Resize(ref stacks, Math.Max(slot + 1, stacks.Length * 2));
            }

            return (Stack<T>)(stacks[slot] ??= new Stack<T>());
        }

        /// <summary>
        /// A growable stack of elements.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        public sealed class Stack<T>
        {
            // ponytail: popped slots keep their references until overwritten; the stacks live as long as one parse.
            private T[] items = new T[16];
            private int count;

            /// <summary>
            /// Gets the number of elements on the stack.
            /// </summary>
            public int Count => count;

            /// <summary>
            /// Drops the elements above <paramref name="count"/>, after a repetition failed.
            /// </summary>
            /// <param name="count">The <see cref="Count"/> to go back to.</param>
            public void Truncate(int count)
                => this.count = count;

            /// <summary>
            /// Pushes an element.
            /// </summary>
            /// <param name="item">The element.</param>
            public void Push(T item)
            {
                if (count == items.Length)
                {
                    Array.Resize(ref items, count * 2);
                }

                items[count++] = item;
            }

            /// <summary>
            /// Pops the elements from <paramref name="start"/> up, in the order they were pushed.
            /// </summary>
            /// <param name="start">The <see cref="Count"/> before the first of them was pushed.</param>
            /// <returns>The elements.</returns>
            public T[] PopFrom(int start)
            {
                var popped = count - start;
                if (popped == 0)
                {
                    return Array.Empty<T>();
                }

                var result = new T[popped];
                Array.Copy(items, start, result, 0, popped);
                count = start;
                return result;
            }
        }

#pragma warning disable S2326, S2743 // One slot per type argument is the point: each closed type gets its own Index.
        private static class Slot<T>
        {
            public static readonly int Index = Interlocked.Increment(ref typeCount) - 1;
        }
#pragma warning restore S2326, S2743
    }
}
