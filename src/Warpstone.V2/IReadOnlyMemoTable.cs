﻿namespace Warpstone.V2;

public interface IReadOnlyMemoTable
{
    public IParseResult? this[int position, IParser parser] { get; }
}