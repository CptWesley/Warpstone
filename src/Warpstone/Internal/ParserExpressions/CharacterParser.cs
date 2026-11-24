using System;
using System.Collections.Generic;
using Warpstone.Internal.ParserImplementations;
using Warpstone.Internal.SourceGeneration;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Represents a parser that parses a character.
    /// </summary>
    internal sealed class CharacterParser : ParserBase<char>
    {
        private static readonly int baseHash = typeof(CharacterParser).GetHashCode() * 31;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterParser"/> class.
        /// </summary>
        /// <param name="character">The character to be parsed.</param>
        public CharacterParser(char character)
        {
            Character = character;
        }

        /// <summary>
        /// The character to be parsed.
        /// </summary>
        public char Character { get; }

        /// <inheritdoc />
        public override IParserImplementation<char> CreateUninitializedImplementation()
            => new CharacterParserImpl(Character);

        /// <inheritdoc />
        public override void EmitSource(SourceEmitter emitter)
        {
            emitter.Open($"if (pos < input.Length && input[pos] == {emitter.Literal(Character)})");
            emitter.Line("end = pos + 1;");
            emitter.Line($"value = {emitter.Literal(Character)};");
            emitter.Line("return true;");
            emitter.Close();
            emitter.Fail($"'{Character}'");
        }

        /// <inheritdoc />
        protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
        {
            // Do nothing.
        }

        /// <inheritdoc />
        public override bool AnalyzeLeftEdge(Func<IParser, bool> isNullable, ICollection<IParser> leftEdge)
        {
            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is CharacterParser other
            && other.Character == Character;

        /// <inheritdoc />
        public override int GetHashCode()
            => baseHash + Character.GetHashCode();
    }
}
