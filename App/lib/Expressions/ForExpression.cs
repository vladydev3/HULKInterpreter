using System.Numerics;

namespace hulk;

public class ForExpression : Expression
{
    public override TokenType Type => TokenType.ForExpression;
    public Token Identifier { get; }
    public RangeFunction? Range { get; }
    public Token? VectorName { get; }
    public Expression Body { get; }

    public ForExpression(Token identifier, RangeFunction? range, Token? vectorName, Expression body)
    {
        Identifier = identifier;
        Range = range;
        VectorName = vectorName;
        Body = body;
    }

    public override object? EvaluateExpression()
    {
        string returnFor = "";

        try
        {
            if (Range != null)
            {
                var lower = Convert.ToInt32(Evaluator.Evaluate(Range.LowerBound));
                var upper = Convert.ToInt32(Evaluator.Evaluate(Range.UpperBound));
                for (int i = lower; i < upper; i++)
                {
                    Evaluator.VariableScope.Add(new Tuple<string, Expression,int>(Identifier.Text, new NumberExpression(new Token(TokenType.Number, i, i.ToString(), double.Parse(i.ToString()))),Evaluator.VariableScope.Count));
                    if (i == upper - 1) returnFor += Evaluator.Evaluate(Body).ToString();
                    else returnFor += Evaluator.Evaluate(Body).ToString() + "\n";
                    Evaluator.VariableScope.Remove(Evaluator.VariableScope.Last());
                }
                return returnFor;
            }
            if (VectorName != null)
            {
                List<object>? vector = new();
                foreach (var item in Evaluator.VariableScope)
                {
                    if (item.Item1 == VectorName.Text)
                    {
                        try
                        {
                            vector = (List<object>?)Evaluator.Evaluate(item.Item2);
                        }
                        catch (Exception)
                        {
                            Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Variable \"{VectorName.Text}\" is not a vector");
                        }
                    }
                }
                for (int i = 0; i < vector.Count; i++)
                {
                    Evaluator.VariableScope.Add(new Tuple<string, Expression,int>(Identifier.Text, new NumberExpression(new Token(TokenType.Number, i, vector[i].ToString(), (double)vector[i])), Evaluator.VariableScope.Count));
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
}