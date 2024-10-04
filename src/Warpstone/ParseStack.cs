namespace Warpstone;

public sealed class ParseStack : IParseStack
{
    private readonly List<object> stack = new();

    public object this[int index] => stack[index];

    public int Count => stack.Count;

    public object? Last => stack.Count == 0 ? null : stack[stack.Count - 1];

    public bool Done => Count == 1 && Last is IParseResult;

    public void Push(object value)
    {
        stack.Add(value);
    }

    public void Pop()
    {
        stack.RemoveAt(stack.Count - 1);
    }

    public IEnumerator<object> GetEnumerator()
        => stack.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
