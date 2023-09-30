
namespace hulk;

public class IfExpression : Expression
{
    public override TokenType Type => TokenType.IfExpression;
    public Expression Condition;
    public Expression ifExpression;
    public Expression ElseExpression;

    public IfExpression(Expression condition, Expression ifexpression, Expression elseExpression)
    {
        Condition = condition;
        ifExpression = ifexpression;
        ElseExpression = elseExpression;
    }

    public override object EvaluateExpression()
    {
        try
        {
            if((bool)Evaluator.Evaluate(Condition))
            {
                return Evaluator.Evaluate(ifExpression);
            }
            return Evaluator.Evaluate(ElseExpression);
        }
        catch (Exception e)
        {
            Diagnostics.AddError("Can't convert the given condition to bool");
        }
        return null;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return Condition;
        yield return ifExpression;
        yield return ElseExpression;
    }
}