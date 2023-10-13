
namespace hulk;

public class WhileExpression : Expression
{
    public override TokenType Type => TokenType.WhileExpression;
    public Expression condition;
    public Expression body;

    public WhileExpression(Expression condition, Expression body)
    {
        this.condition = condition;
        this.body = body;
    }

    public override object EvaluateExpression()
    {
        string returnWhile = "";
        while (Evaluator.Evaluate(condition) is bool conditionValue && conditionValue)
        {
            var evaluatedBody = Evaluator.Evaluate(body);
            if (evaluatedBody != null)
            {
                returnWhile += evaluatedBody.ToString() + "\n";
            }
        }
        return returnWhile;
    }
}