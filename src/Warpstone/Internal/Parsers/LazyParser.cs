using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warpstone.Internal.Parsers;

public sealed record LazyParser<T>(Lazy<IParser<T>> Element) : IParser<T>
{
    public Type ResultType => typeof(T);

    public LazyParser(Func<IParser<T>> get)
        : this(new Lazy<IParser<T>>(get))
    {
    }

    public void Apply(IIterativeParseContext context, int position)
    {
        context.ExecutionStack.Push((position, Element.Value));
    }

    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        return Element.Value.Apply(context, position);
    }
}
