using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.SourceGeneration
{
    /// <summary>
    /// A parser expression that can write the C# source that runs it.
    /// </summary>
    internal interface IEmittableParser : IParser
    {
        /// <summary>
        /// Creates the plan that decides which parsers are memoized, grown or guarded for the given <paramref name="options"/>.
        /// </summary>
        /// <param name="options">The parse options.</param>
        /// <returns>The plan.</returns>
        public ParserPlan CreatePlan(ParseOptions options);

        /// <summary>
        /// Writes the body of the method running this parser: <c>bool P(int pos, out int end, out T value)</c>.
        /// </summary>
        /// <param name="emitter">The emitter to write to.</param>
        public void EmitSource(SourceEmitter emitter);
    }
}
