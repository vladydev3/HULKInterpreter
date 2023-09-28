namespace hulk;

class Evaluator
{
    public static Errors Diagnostics = new();
    public static List<Tuple<string,Expression>> VariableScope = new();

    static public object Evaluate(Expression node)
    {
        var result = node.EvaluateExpression();
        Diagnostics = node.Diagnostics;

        return result;
    }
}