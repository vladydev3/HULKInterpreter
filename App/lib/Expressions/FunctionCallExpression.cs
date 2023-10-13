using System.Reflection.Metadata.Ecma335;

namespace hulk;

public class FunctionCallExpression : Expression
{
    public override TokenType Type => TokenType.FunctionExpression;
    public Token Name;
    public List<Expression> Arguments;

    public FunctionCallExpression(Token name, List<Expression> arguments)
    {
        Name = name;
        Arguments = arguments;
    }

    public override object? EvaluateExpression()
    {

        if (Evaluator.VariableScope.Count > 1000)
        {
            Evaluator.Diagnostics.AddError("! Stack Overflow.");
            return null;
        }

        object? toReturn = null;
        int count = 0;
        foreach (var item in Evaluator.FunctionsScope)
        {
            List<Tuple<string, Expression, int>> temp = new();
            if (item.Item1 == Name.Text)
            {
                if (item.Item2.Count != Arguments.Count)
                {
                    Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Function \"{Name.Text}\" receives {item.Item2.Count} argument(s), but {Arguments.Count} were given (column {Name.Position + 2}).");
                    return null;
                }

                for (int i = 0; i < Arguments.Count; i++)
                {
                    var argument = Arguments[i].EvaluateExpression();
                    if (argument == null)
                    {
                        Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Missing argument (column {Name.Position + 2}).");
                        return null;
                    }
                    var a = new Parser(argument.ToString());
                    var exp = a.ParseExpression();
                    if (exp == null)
                    {
                        Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Missing argument (column {Name.Position + 2}).");
                        return null;
                    }

                    var func = InferenceTypes.GetInferenceType(item.Item3);
                    var arg = InferenceTypes.GetInferenceType(argument);

                    if (func != arg && func != InferenceType.Any)
                    {
                        Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Function \"{Name.Text}\" receives {InferenceTypes.GetInferenceType(item.Item3)} argument, but {InferenceTypes.GetInferenceType(argument)} was given (column {Name.Position + 1}).");
                        return null;
                    }

                    temp.Add(new Tuple<string, Expression, int>(item.Item2[i].Text, exp, Evaluator.VariableScope.Count));
                    count++;
                }
                Evaluator.VariableScope.AddRange(temp);
                toReturn = Evaluator.Evaluate(item.Item3);
            }
        }
        if (toReturn == null)
        {
            Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Function \"{Name.Text}\" is not defined (column {Name.Position}).");
            return null;
        }
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                Evaluator.VariableScope.RemoveAt(Evaluator.VariableScope.Count - 1);
            }
        }
        
        return toReturn;
    }
}