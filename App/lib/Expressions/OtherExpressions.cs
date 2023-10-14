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

    public override object? EvaluateExpression()
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

    public override object? EvaluateExpression()
    {
        try
        {
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

    public override object? EvaluateExpression()
    {
        switch (MathFunc.Text)
        {
            case "sin":
                try
                {
                    return Math.Sin(Convert.ToDouble(Evaluator.Evaluate(ExpressionInside)));
                }
                catch (Exception e)
                {
                    Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
                }
                break;

            case "cos":
                try
                {
                    return Math.Cos(Convert.ToDouble(Evaluator.Evaluate(ExpressionInside)));
                }
                catch (Exception e)
                {
                    Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
                }
                break;
            case "sqrt":
                try
                {
                    return Math.Sqrt(Convert.ToDouble(Evaluator.Evaluate(ExpressionInside)));
                }
                catch (Exception e)
                {
                    Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
                }
                break;
            case "exp":
                try
                {
                    return Math.Pow(Math.E, Convert.ToDouble(Evaluator.Evaluate(ExpressionInside)));
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
        }
        return null;
    }
}

public class LogExpression : Expression
{
    public override TokenType Type => TokenType.MathFunctions;
    public Expression Base { get; }
    public Expression Number { get; }

    public LogExpression(Expression b, Expression number)
    {
        Base = b;
        Number = number;
    }

    public override object? EvaluateExpression()
    {
        try
        {
            double? a = Convert.ToDouble(Number.EvaluateExpression());
            double? newBase = Convert.ToDouble(Base.EvaluateExpression());
            if (newBase == 1)
            {
                Diagnostics.AddError($"! SEMANTIC ERROR: Logarithm base cannot be 1");
                return null;
            }
            if (a == 0)
            {
                Diagnostics.AddError($"! SEMANTIC ERROR: Logarithm of 0 is undefined");
                return null;
            }
            if (a != null && newBase != null) return Math.Log(a.Value, newBase.Value);
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

    public override object? EvaluateExpression()
    {
        try
        {
            var next = VectorExpression.GetVector(Name);
            if (next != null) return next.GetNext();
            return null;
        }
        catch (Exception e)
        {
            Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
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

    public override object? EvaluateExpression()
    {
        try
        {
            var vector = VectorExpression.GetVector(Name);
            if (vector != null) return vector.GetCurrent(Name);
            return null;
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

        var lower = LowerBound.EvaluateExpression();
        var upper = UpperBound.EvaluateExpression();

        if (upper == null || lower == null) return vector;
        for (int i = Convert.ToInt32(lower); i < Convert.ToInt32(upper); i++)
        {
            vector.Add(new NumberExpression(new Token(TokenType.Number, 0, "", i)));
        }
        return vector;
    }

    public override object? EvaluateExpression()
    {
        try
        {
            var lower = Convert.ToDouble(Evaluator.Evaluate(LowerBound));
            var upper = Convert.ToDouble(Evaluator.Evaluate(UpperBound));
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