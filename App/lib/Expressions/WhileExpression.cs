
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
        while ((bool)Evaluator.Evaluate(condition))
        {
            returnWhile += Evaluator.Evaluate(body).ToString() + "\n";
        }
        return returnWhile;
    }
}