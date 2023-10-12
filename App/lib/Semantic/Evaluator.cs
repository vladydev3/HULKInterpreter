namespace hulk;

public class Evaluator
{
    public static Errors Diagnostics = new();
    public static List<Tuple<string,Expression, int>> VariableScope = new();
    public static List<Tuple<string, List<Token>, Expression>> FunctionsScope = new();
    public static int StackPointer = 0;
    public static int ScopePointer = 0;
    public static bool PrintResult = false;

    static public object Evaluate(Expression node)
    {
        var result = node.EvaluateExpression();

        return result;
    }
}