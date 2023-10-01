namespace hulk;

public class Evaluator
{
    public static Errors Diagnostics = new();
    public static List<Tuple<string,Expression>> VariableScope = new();
    public static List<Tuple<string, List<Token>, Expression>> FunctionsScope = new();

    static public object Evaluate(Expression node)
    {
        var result = node.EvaluateExpression();

        return result;
    }
}