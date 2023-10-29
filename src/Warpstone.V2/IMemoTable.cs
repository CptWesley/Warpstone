﻿using Warpstone.V2.Parsers;

namespace Warpstone.V2;

public interface IMemoTable : IReadOnlyMemoTable
{
    public new IParseResult? this[int position, IParser parser] { get; set; }
}
