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
        foreach (var item in Evaluator.FunctionsScope)
        {
            if (item.Item1 == Name.Text)
            {
                if (item.Item2.Count != Arguments.Count)
                {
                    Evaluator.Diagnostics.AddError($"Semantic Error: Function \"{Name.Text}\" does not take {Arguments.Count} arguments.");
                    return null;
                }
                
                for (int i=0; i<Arguments.Count; i++)
                {
                    Evaluator.VariableScope.Add(new Tuple<string, Expression>(item.Item2[i].Text, Arguments[i]));
                }

                return Evaluator.Evaluate(item.Item3);
            }
        }
        Evaluator.Diagnostics.AddError($"Semantic Error: Function \"{Name.Text}\" is not defined.");
        return null;
    }

    public override IEnumerable<Node> GetChildren()
    {
        throw new NotImplementedException();
    }
}