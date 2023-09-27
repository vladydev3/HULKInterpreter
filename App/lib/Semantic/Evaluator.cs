namespace hulk;

class Evaluator
{
    static public Errors Diagnostics = new();

    static public object Evaluate(Expression node)
    {
        var result = node.EvaluateExpression();
        Diagnostics = node.Diagnostics;

        return result;
    }
}