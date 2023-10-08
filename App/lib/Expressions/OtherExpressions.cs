namespace hulk;

public sealed class ParenExpression : Expression
{
    public override TokenType Type => TokenType.Parenthesis;
    public Token OpenParen { get; }
    public Expression Expression { get; }
    public Token CloseParen { get; }

    public ParenExpression(Token openParen, Expression expression, Token closeParen)
    {
        OpenParen = openParen;
        Expression = expression;
        CloseParen = closeParen;
    }

    public override object EvaluateExpression()
    {
        return Evaluator.Evaluate(Expression);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return OpenParen;
        yield return Expression;
        yield return CloseParen;
    }
}

public class PrintExpression : Expression
{
    public override TokenType Type => TokenType.PrintExpression;
    public Token PrintToken;
    public ParenExpression ExpressionInside;

    public PrintExpression(Token printToken, Token openParen, Expression expression, Token closeParen)
    {
        PrintToken = printToken;
        ExpressionInside = new ParenExpression(openParen, expression, closeParen);
    }

    public override object EvaluateExpression()
    {
        try
        {
            Evaluator.PrintResult = true;
            return Evaluator.Evaluate(ExpressionInside);
        }
        catch (Exception e)
        {
            return "";
        }
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return ExpressionInside;
    }
}

public sealed class MathExpression : Expression
{
    public override TokenType Type => TokenType.MathFunctions;
    public Token MathFunc { get; }
    public ParenExpression ExpressionInside { get; }

    public MathExpression(Token mathFunc, Token openParen, Expression expression, Token closeParen)
    {
        MathFunc = mathFunc;
        ExpressionInside = new ParenExpression(openParen, expression, closeParen);
    }

    public override object EvaluateExpression()
    {
        switch (MathFunc.Text)
        {
            case "sin":
                try
                {
                    return Math.Sin((double)Evaluator.Evaluate(ExpressionInside));
                }
                catch (Exception e)
                {
                    Diagnostics.AddError($"Semantic Error: {e.Message}");
                }
                break;

            case "cos":
                try
                {
                    return Math.Cos((double)Evaluator.Evaluate(ExpressionInside));
                }
                catch (Exception e)
                {
                    Diagnostics.AddError($"Semantic Error: {e.Message}");
                }
                break;
            case "sqrt":
                try
                {
                    return Math.Sqrt((double)Evaluator.Evaluate(ExpressionInside));
                }
                catch (Exception e)
                {
                    Diagnostics.AddError($"Semantic Error: {e.Message}");
                }
                break;
            case "exp":
                try
                {
                    return Math.Pow(Math.E, (double)Evaluator.Evaluate(ExpressionInside));
                }
                catch (Exception e)
                {
                    Diagnostics.AddError($"Semantic Error: {e.Message}");
                }
                break;
        }
        return null;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return MathFunc;
        yield return ExpressionInside;
    }
}

public sealed class LogExpression : Expression
{
    public override TokenType Type => TokenType.MathFunctions;
    public Token LogFunc { get; }
    public Token Base { get; }
    public Token Number { get; }

    public LogExpression(Token logFunc, Token b, Token number)
    {
        LogFunc = logFunc;
        Base = b;
        Number = number;
    }

    public override object EvaluateExpression()
    {
        try
        {
            return Math.Log((double)Number.Value, (double)Base.Value);
        }
        catch (Exception e)
        {
            Diagnostics.AddError($"Semantic Error: {e.Message}");
        }
        return null;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return LogFunc;
        yield return Base;
        yield return Number;
    }

}
