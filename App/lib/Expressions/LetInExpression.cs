namespace hulk;

public sealed class LetInExpression : Expression
{
    public override TokenType Type => TokenType.LetInExpression;
    public Token VariableName;
    public Expression Value;
    public Expression InExpression;

    public LetInExpression(Token variableName, Expression value, Expression inExpression)
    {
        VariableName = variableName;
        Value = value;
        InExpression = inExpression;
    }

    public override object EvaluateExpression()
    {
        return Evaluator.Evaluate(InExpression);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return VariableName;
        yield return Value;
        yield return InExpression;
    }
}

