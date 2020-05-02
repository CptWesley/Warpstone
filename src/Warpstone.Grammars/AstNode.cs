using System;
using System.Collections.Generic;
using System.Linq;

namespace Warpstone.Grammars
{
    /// <summary>
    /// Generic parsed AST node created by the parsers generated from grammars.
    /// </summary>
    /// <seealso cref="IParsed" />
    public abstract class AstNode : IParsed, IEquatable<AstNode>
    {
        /// <inheritdoc/>
        public SourcePosition Position { get; set; }

        /// <inheritdoc/>
        public abstract bool Equals(AstNode other);

        /// <inheritdoc/>
        public abstract override bool Equals(object obj);

        /// <inheritdoc/>
        public abstract override int GetHashCode();

        /// <inheritdoc/>
        public abstract override string ToString();
    }

    /// <summary>
    /// AST node for string literals.
    /// </summary>
    /// <seealso cref="AstNode" />
    /// <seealso cref="IEquatable{StringNode}" />
    public sealed class StringNode : AstNode, IEquatable<StringNode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringNode"/> class.
        /// </summary>
        /// <param name="str">The string.</param>
        public StringNode(string str)
            => Value = str;

        /// <summary>
        /// Gets the string value.
        /// </summary>
        public string Value { get; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is StringNode sn)
            {
                return Equals(sn);
            }

            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(AstNode other)
        {
            if (other is StringNode sn)
            {
                return Equals(sn);
            }

            return false;
        }

        /// <inheritdoc/>
        public bool Equals(StringNode other)
        {
            if (other == null)
            {
                return false;
            }

            return Value == other.Value && Position == other.Position;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
            => Position.GetHashCode() * (Value is null ? 1 : Value.GetHashCode());

        /// <inheritdoc/>
        public override string ToString()
            => Value is null ? "null" : $"\"{Value}\"";

        /// <summary>
        /// Deconstructs the node into it's string equivalent.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Deconstruct(out string value)
            => value = Value;
    }

    /// <summary>
    /// AST nodes for objects.
    /// </summary>
    /// <seealso cref="AstNode" />
    /// <seealso cref="IEquatable{ObjectNode}" />
    public sealed class ObjectNode : AstNode, IEquatable<ObjectNode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectNode"/> class.
        /// </summary>
        /// <param name="type">The type name.</param>
        /// <param name="children">The children.</param>
        public ObjectNode(string type, IEnumerable<AstNode> children)
        {
            Type = type;
            Children = Children = children.ToArray();
        }

        /// <summary>
        /// Gets the type name.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        public IReadOnlyList<AstNode> Children { get; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is ObjectNode on)
            {
                return Equals(on);
            }

            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(AstNode other)
        {
            if (other is ObjectNode on)
            {
                return Equals(on);
            }

            return false;
        }

        /// <inheritdoc/>
        public bool Equals(ObjectNode other)
        {
            if (other == null || Type != other.Type || Position != other.Position || Children.Count != other.Children.Count)
            {
                return false;
            }

            for (int i = 0; i < Children.Count; i++)
            {
                if (!Children[i].Equals(other.Children[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hash = Position.GetHashCode() + Type.GetHashCode();

            for (int i = 0; i < Children.Count; i++)
            {
                hash += (i + 1) * Children[i].GetHashCode();
            }

            return hash;
        }

        /// <inheritdoc/>
        public override string ToString()
            => $"{Type}({string.Join(", ", Children)})";

        /// <summary>
        /// Deconstructs the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="children">The children.</param>
        public void Deconstruct(out string type, out IEnumerable<AstNode> children)
        {
            type = Type;
            children = Children;
        }
    }
}
