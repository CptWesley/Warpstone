using System.Collections;
using System.Collections.Generic;

namespace Warpstone;

public interface IParseStack : IReadOnlyParseStack
{
    public bool Done { get; }

    public void Push(object value);

    public void Pop();
}
