namespace hulk;

public class ForExpression : Expression
{
    public override TokenType Type => TokenType.ForExpression;
    public Token Identifier { get; }
    public RangeExpression Range { get; }
    public Expression Body { get; }

    public ForExpression(Token identifier, RangeExpression range, Expression body)
    {
        Identifier = identifier;
        Range = range;
        Body = body;
    }

    public override object EvaluateExpression()
    {
        string returnFor = "";
        Evaluator.PrintResult = true;

        try
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