
namespace hulk;

public class IfExpression : Expression
{
    public override TokenType Type => TokenType.IfExpression;
    public Expression Condition;
    public Expression ifExpression;
    public List<Expression> ElifCondition;
    public List<Expression> ElifExpression;
    public Expression ElseExpression;

    public IfExpression(Expression condition, Expression ifexpression, List<Expression> elifCondition, List<Expression> elifExpression, Expression elseExpression)
    {
        Condition = condition;
        ifExpression = ifexpression;
        ElifCondition = elifCondition;
        ElifExpression = elifExpression;
        ElseExpression = elseExpression;
    }

    public override object EvaluateExpression()
    {
        try
        {
            if ((bool)Evaluator.Evaluate(Condition))
            {
                return Evaluator.Evaluate(ifExpression);
            }
            for (int i = 0; i < ElifCondition.Count; i++)
            {
                if ((bool)Evaluator.Evaluate(ElifCondition[i]))
                {
                    return Evaluator.Evaluate(ElifExpression[i]);
                }
            }
            return Evaluator.Evaluate(ElseExpression);
        }
        catch (Exception e)
        {
            Diagnostics.AddError("Can't convert the given condition to bool");
        }
        return null;
    }
}