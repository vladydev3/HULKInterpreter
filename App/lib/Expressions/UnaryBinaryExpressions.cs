namespace hulk;

public sealed class UnaryExpression : Expression
{

    public override TokenType Type => TokenType.UnaryExpression;
    public Token OperatorToken { get; }
    public Expression Operand { get; }
    public UnaryExpression(Token operatorToken, Expression operand)
    {
        OperatorToken = operatorToken;
        Operand = operand;
    }

    public override object EvaluateExpression()
    {
        var operand = Evaluator.Evaluate(Operand);

        if (OperatorToken.Type == TokenType.Negation)
        {
            try
            {
                return !(bool)operand;
            }
            catch (Exception e)
            {
                Diagnostics.AddError($"Semantic Error: {e.Message}");
            }
        }
        else if (OperatorToken.Type == TokenType.Minus)
        {
            try
            {
                return -(double)operand;
            }
            catch (Exception e)
            {
                Diagnostics.AddError($"Semantic Error: {e.Message}");
            }
        }
        else if (OperatorToken.Type == TokenType.Plus) return operand;
        return operand;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return OperatorToken;
        yield return Operand;
    }
}

public sealed class BinaryExpression : Expression
{
    public override TokenType Type => TokenType.BinaryExpression;
    public Expression Left { get; }
    public Token Operator { get; }
    public Expression Right { get; }

    public BinaryExpression(Expression left, Token operatorToken, Expression right)
    {
        Left = left;
        Operator = operatorToken;
        Right = right;
    }

    public override object EvaluateExpression()
    {
        object? left = Evaluator.Evaluate(Left);
        object? right = Evaluator.Evaluate(Right);

        if (Operator.Type == TokenType.Plus)
        {
            try
            {
                return (double)left + (double)right;
            }
            catch (Exception e)
            {
                Diagnostics.AddError($"Semantic Error: {e.Message}");
            }
        }
        else if (Operator.Type == TokenType.Minus)
        {
            try
            {
                return (double)left - (double)right;
            }
            catch (Exception e)
            {
                Diagnostics.AddError($"Semantic Error: {e.Message}");
            }
        }
        else if (Operator.Type == TokenType.Mult)
        {
            try
            {
                return (double)left * (double)right;
            }
            catch (Exception e)
            {
                Diagnostics.AddError($"Semantic Error: {e.Message}");
            }
        }
        else if (Operator.Type == TokenType.Div)
        {
            try
            {
                return (double)left / (double)right;
            }
            catch (Exception e)
            {
                Diagnostics.AddError($"Semantic Error: {e.Message}");
            }
        }
        else if (Operator.Type == TokenType.Mod)
        {
            try
            {
                return (double)left % (double)right;
            }
            catch (Exception e)
            {
                Diagnostics.AddError($"Semantic Error: {e.Message}");
            }
        }
        else if (Operator.Type == TokenType.Concat)
        {
            try
            {
                return (string)left + right;
            }
            catch (Exception e)
            {
                Diagnostics.AddError($"Semantic Error: At least one of the elements to concatenate must be a string");
            }
            try
            {
                return left + (string)right;
            }
            catch (Exception e)
            {
                Diagnostics.AddError($"Semantic Error: At least one of the elements to concatenate must be a string");
            }
        }
        else if (Operator.Type == TokenType.Pow)
        {
            try
            {
                return Math.Pow((double)left, (double)right);
            }
            catch (Exception e)
            {
                Diagnostics.AddError($"Semantic Error: {e.Message}");
            }
        }
        else if (Operator.Type == TokenType.Comparation)
        {
            try
            {
                return (double)left == (double)right;
            }
            catch (Exception e)
            {
                Diagnostics.AddError($"Semantic Error: {$"Semantic Error: {e.Message}"}");
            }

            try
            {
                return (bool)left == (bool)right;
            }
            catch (Exception e)
            {
                Diagnostics.AddError($"Semantic Error: {$"Semantic Error: {e.Message}"}");
            }
        }
        else if (Operator.Type == TokenType.Bigger)
        {
            try
            {
                return (double)left > (double)right;
            }
            catch (Exception e)
            {
                Diagnostics.AddError($"Semantic Error: {e.Message}");
            }
        }
        else if (Operator.Type == TokenType.Minor)
        {
            try
            {
                return (double)left < (double)right;
            }
            catch (Exception e)
            {
                Diagnostics.AddError($"Semantic Error: {e.Message}");
            }
        }
        else if (Operator.Type == TokenType.BiggerOrEqual)
        {
            try
            {
                return (double)left >= (double)right;
            }
            catch (Exception e)
            {
                Diagnostics.AddError($"Semantic Error: {e.Message}");
            }
        }
        else if (Operator.Type == TokenType.MinorOrEqual)
        {
            try
            {
                return (double)left <= (double)right;
            }
            catch (Exception e)
            {
                Diagnostics.AddError($"Semantic Error: {e.Message}");
            }
        }
        
        return null;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return Left;
        yield return Operator;
        yield return Right;
    }
}