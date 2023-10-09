using System.Numerics;

namespace hulk;

public class ForExpression : Expression
{
    public override TokenType Type => TokenType.ForExpression;
    public Token Identifier { get; }
    public RangeExpression Range { get; }
    public Token VectorName { get; }
    public Expression Body { get; }

    public ForExpression(Token identifier, RangeExpression range, Token vectorName, Expression body)
    {
        Identifier = identifier;
        Range = range;
        VectorName = vectorName;
        Body = body;
    }

    public override object EvaluateExpression()
    {
        string returnFor = "";
        Evaluator.PrintResult = true;

        try
        {
            if (Range != null)
            {
                var lower = int.Parse(Evaluator.Evaluate(Range.LowerBound).ToString());
                var upper = int.Parse(Evaluator.Evaluate(Range.UpperBound).ToString());
                for (int i = lower; i < upper; i++)
                {
                    Evaluator.VariableScope.Add(new Tuple<string, Expression>(Identifier.Text, new NumberExpression(new Token(TokenType.Number, i, i.ToString(), double.Parse(i.ToString())))));
                    if (i == upper - 1) returnFor += Evaluator.Evaluate(Body).ToString();
                    else returnFor += Evaluator.Evaluate(Body).ToString() + "\n";
                    Evaluator.VariableScope.Remove(Evaluator.VariableScope.Last());
                }
                return returnFor;
            }
            else
            {
                List<object> vector = new();
                foreach (var item in Evaluator.VariableScope)
                {
                    if (item.Item1 == VectorName.Text)
                    {
                        try
                        {
                            vector = (List<object>)Evaluator.Evaluate(item.Item2);
                        }
                        catch (Exception)
                        {
                            Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Variable \"{VectorName.Text}\" is not a vector");
                        }
                    }
                }
                for (int i = 0; i < vector.Count; i++)
                {
                    Evaluator.VariableScope.Add(new Tuple<string, Expression>(Identifier.Text, new NumberExpression(new Token(TokenType.Number, i, vector[i].ToString(), double.Parse(vector[i].ToString())))));
                    if (i == vector.Count - 1) returnFor += Evaluator.Evaluate(Body).ToString();
                    else returnFor += Evaluator.Evaluate(Body).ToString() + "\n";
                    Evaluator.VariableScope.Remove(Evaluator.VariableScope.Last());
                }
                return returnFor;
            }
        }
        catch (Exception e)
        {
            Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
        }
        return null;
    }


    public override IEnumerable<Node> GetChildren()
    {
        yield return Identifier;
        yield return Body;
    }
}