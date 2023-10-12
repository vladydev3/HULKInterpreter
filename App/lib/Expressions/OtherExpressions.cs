namespace hulk;

public class ParenExpression : Expression
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
}

public class PrintExpression : Expression
{
    public override TokenType Type => TokenType.PrintExpression;
    public Expression ExpressionInside;

    public PrintExpression(Expression expression)
    {
        ExpressionInside = expression;
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
            Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
            return null;
        }
    }
}

public class MathExpression : Expression
{
    public override TokenType Type => TokenType.MathFunctions;
    public Token MathFunc { get; }
    public Expression ExpressionInside { get; }

    public MathExpression(Token mathFunc, Expression expression)
    {
        MathFunc = mathFunc;
        ExpressionInside = expression;
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
                    Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
                }
                break;

            case "cos":
                try
                {
                    return Math.Cos((double)Evaluator.Evaluate(ExpressionInside));
                }
                catch (Exception e)
                {
                    Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
                }
                break;
            case "sqrt":
                try
                {
                    return Math.Sqrt(double.Parse(Evaluator.Evaluate(ExpressionInside).ToString()));
                }
                catch (Exception e)
                {
                    Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
                }
                break;
            case "exp":
                try
                {
                    return Math.Pow(Math.E, (double)Evaluator.Evaluate(ExpressionInside));
                }
                catch (Exception e)
                {
                    Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
                }
                break;
            case "PI":
                try
                {
                    return Math.PI;
                }
                catch (Exception e)
                {
                    Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
                }
                break;
            case "E":
                try
                {
                    return Math.E;
                }
                catch (Exception e)
                {
                    Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
                }
                break;
            case "rand":
                try
                {
                    return new Random().NextDouble();
                }
                catch (Exception e)
                {
                    Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
                }
                break;
        }
        return null;
    }
}

public class LogExpression : Expression
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
            Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
        }
        return null;
    }
}

public class NextFunction : Expression
{
    public override TokenType Type => TokenType.FunctionExpression;
    public Token Name;

    public NextFunction(Token name)
    {
        Name = name;
    }

    public override object EvaluateExpression()
    {
        try
        {
            return VectorExpression.GetVector(Name).GetNext();
        }
        catch (Exception e)
        {
            Evaluator.Diagnostics.AddError($"! ! SEMANTIC ERROR: {e.Message}");
            return null;
        }
    }
}

public class CurrentFunction : Expression
{
    public override TokenType Type => TokenType.FunctionExpression;
    public Token Name;

    public CurrentFunction(Token name)
    {
        Name = name;
    }

    public override object EvaluateExpression()
    {
        try
        {
            return VectorExpression.GetVector(Name).GetCurrent(Name);
        }
        catch (Exception e)
        {
            Evaluator.Diagnostics.AddError($"! ! SEMANTIC ERROR: {e.Message}");
            return null;
        }
    }

}

public class RangeFunction : Expression
{
    public override TokenType Type => TokenType.RangeFunction;
    public Expression LowerBound { get; }
    public Expression UpperBound { get; }

    public RangeFunction(Expression lowerBound, Expression upperBound)
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
    }

    public List<Expression> GetVector()
    {
        List<Expression> vector = new();

        for (int i = int.Parse(LowerBound.EvaluateExpression().ToString()); i < int.Parse(UpperBound.EvaluateExpression().ToString()); i++)
        {
            vector.Add(new NumberExpression(new Token(TokenType.Number, 0, "", i)));
        }
        return vector;
    }

    public override object EvaluateExpression()
    {
        try
        {
            var lower = (double)Evaluator.Evaluate(LowerBound);
            var upper = (double)Evaluator.Evaluate(UpperBound);
            var range = new List<double>((int)(upper - lower));
            for (var i = lower; i < upper; i++)
            {
                range.Add(i);
            }
            return range;
        }
        catch (Exception e)
        {
            Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
        }
        return null;
    }
}