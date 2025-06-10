global using Warpstone.Errors;
global using Warpstone.Internal;
global using Warpstone.Internal.ParserExpressions;
global using Warpstone.Internal.ParserImplementations;

#if !NET9_0_OR_GREATER
global using Lock = System.Object;
#endif
