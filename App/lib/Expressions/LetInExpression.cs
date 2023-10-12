namespace hulk;

public sealed class LetInExpression : Expression
{
    public override TokenType Type => TokenType.LetInExpression;
    public List<Token> VariablesNames;
    public List<Expression> Values;
    public Expression InExpression;

    public LetInExpression(List<Token> variablesNames, List<Expression> values, Expression inExpression)
    {
        VariablesNames = variablesNames;
        Values = values;
        InExpression = inExpression;
    }

    public override object EvaluateExpression()
    {
        return Evaluator.Evaluate(InExpression);
    }
}

