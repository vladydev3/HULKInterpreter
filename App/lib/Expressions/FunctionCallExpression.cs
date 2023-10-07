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
        object toReturn = null;
        foreach (var item in Evaluator.FunctionsScope)
        {
            if (item.Item1 == Name.Text)
            {
                if (item.Item2.Count != Arguments.Count)
                {
                    Evaluator.Diagnostics.AddError($"Semantic Error: Function \"{Name.Text}\" take {item.Item2.Count} arguments, not {Arguments.Count}.");
                    return null;
                }

                for (int i = 0; i < Arguments.Count; i++)
                {
                    var a = new Parser(Arguments[i].EvaluateExpression().ToString());
                    var exp = a.ParseExpression();
                    Evaluator.VariableScope.Add(new Tuple<string, Expression>(item.Item2[i].Text, exp));
                }

                toReturn = Evaluator.Evaluate(item.Item3);
            }
        }
        if (toReturn == null)
        {
            Evaluator.Diagnostics.AddError($"Semantic Error: Function \"{Name.Text}\" is not defined.");
            return null;
        }
        if (Evaluator.VariableScope.Count > 0) Evaluator.VariableScope.RemoveAt(Evaluator.VariableScope.Count-1);
        return toReturn;
    }

    public override IEnumerable<Node> GetChildren()
    {
        throw new NotImplementedException();
    }
}