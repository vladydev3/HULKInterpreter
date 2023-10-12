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

    public override object EvaluateExpression()
    {
        if (Evaluator.StackPointer >= 1000)
        {
            Evaluator.Diagnostics.AddError("! Stack Overflow.");
            return null;
        }
        Evaluator.StackPointer++;
        object toReturn = null;
        int count = 0;
        foreach (var item in Evaluator.FunctionsScope)
        {
            List<Tuple<string, Expression, int>> temp = new();
            if (item.Item1 == Name.Text)
            {
                if (item.Item2.Count != Arguments.Count)
                {
                    Evaluator.Diagnostics.AddError($"Semantic Error: Function \"{Name.Text}\" receives {item.Item2.Count} arguments, but {Arguments.Count} were given.");
                    return null;
                }

                for (int i = 0; i < Arguments.Count; i++)
                {
                    var argument = Arguments[i].EvaluateExpression();
                    var a = new Parser(argument.ToString());
                    var exp = a.ParseExpression();

                    var func = InferenceTypes.GetInferenceType(item.Item3);
                    var arg = InferenceTypes.GetInferenceType(argument);

                    if (func != arg && func != InferenceType.Any)
                    {
                        Evaluator.Diagnostics.AddError($"Semantic Error: Function \"{Name.Text}\" receives {InferenceTypes.GetInferenceType(item.Item3)} argument, but {InferenceTypes.GetInferenceType(argument)} was given.");
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
            Evaluator.Diagnostics.AddError($"Semantic Error: Function \"{Name.Text}\" is not defined.");
            return null;
        }
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                Evaluator.VariableScope.RemoveAt(Evaluator.VariableScope.Count - 1);
            }
        }
        Evaluator.PrintResult = true;
        return toReturn;
    }

    public override IEnumerable<Node> GetChildren()
    {
        throw new NotImplementedException();
    }
}