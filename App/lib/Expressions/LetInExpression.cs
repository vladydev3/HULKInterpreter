namespace hulk;

public sealed class LetInExpression : Expression
{
    public override TokenType Type => TokenType.LetInExpression;
    public Token VariableName1;
    public Token VariableName2;
    public Expression Value1;
    public Expression Value2;
    public Expression InExpression;

    public LetInExpression(Token variableName1, Token variableName2, Expression value1, Expression value2, Expression inExpression)
    {
        VariableName1 = variableName1;
        VariableName2 = variableName2;
        Value1 = value1;
        Value2 = value2;
        InExpression = inExpression;
    }

    public override object EvaluateExpression()
    {
        return Evaluator.Evaluate(InExpression);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return VariableName1;
        yield return VariableName2;
        yield return Value1;
        yield return Value2;
        yield return InExpression;
    }
}

