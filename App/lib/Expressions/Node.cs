namespace hulk;

public abstract class Node
{
    public abstract TokenType Type { get; }
    public abstract IEnumerable<Node> GetChildren();
}

public abstract class Expression : Node
{
    public abstract object EvaluateExpression();
    public Errors Diagnostics = new();
}

