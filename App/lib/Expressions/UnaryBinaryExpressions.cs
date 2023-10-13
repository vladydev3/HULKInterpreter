namespace hulk;

public class UnaryExpression : Expression
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
                Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
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
                Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
            }
        }
        else if (OperatorToken.Type == TokenType.Plus) return operand;
        return operand;
    }

}

public class BinaryExpression : Expression
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
        object left = Evaluator.Evaluate(Left);
        object right = Evaluator.Evaluate(Right);

        if (Operator.Type == TokenType.Plus)
        {
            try
            {
                return (double)left + (double)right;
            }
            catch (Exception)
            {
                if (right != null && left != null) Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Operator '+' cannot be used between '{left.GetType()}' and '{right.GetType()}'.");
                else Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Operator '+' only can be used between two numbers");
            }
        }
        else if (Operator.Type == TokenType.Minus)
        {
            try
            {
                return (double)left - (double)right;
            }
            catch (Exception)
            {
                if (right != null && left != null) Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Operator '-' cannot be used between '{left.GetType()}' and '{right.GetType()}'.");
                else Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Operator '-' only can be used between two numbers");
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
                if (right != null && left != null) Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Operator '*' cannot be used between '{left.GetType()}' and '{right.GetType()}'.");
                else Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Operator '*' only can be used between two numbers");
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
                if (right != null && left != null) Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Operator '/' cannot be used between '{left.GetType()}' and '{right.GetType()}'.");
                else Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Operator '/' only can be used between two numbers");
            }
        }
        else if (Operator.Type == TokenType.Mod)
        {
            try
            {
                return (double)left % (double)right;
            }
            catch (Exception)
            {
                if (right != null && left != null) Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Operator '%' cannot be used between '{left.GetType()}' and '{right.GetType()}'.");
                else Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Operator '%' only can be used between two numbers");
            }
        }
        else if (Operator.Type == TokenType.Concat)
        {
            try
            {
                return (string)left + right;
            }
            catch (Exception)
            {
                Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: At least one of the elements to concatenate must be a string");
            }
            try
            {
                var toReturn = left + (string)right;
                Evaluator.Diagnostics.RemoveError();
                return toReturn;
            }
            catch (Exception)
            {
                Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: At least one of the elements to concatenate must be a string");
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
                Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
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
                Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
            }

            try
            {
                var toReturn = (bool)left == (bool)right;
                Evaluator.Diagnostics.RemoveError();
                return toReturn;
            }
            catch (Exception e)
            {
                Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
            }
            try
            {
                var toReturn = (string)left == (string)right;
                Evaluator.Diagnostics.RemoveError();
                Evaluator.Diagnostics.RemoveError();
                return toReturn;
            }
            catch (Exception e)
            {
                Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
            }
        }
        else if (Operator.Type == TokenType.Diferent)
        {
            try
            {
                return (bool)left != (bool)right;
            }
            catch (Exception e)
            {
                Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
            }
            try
            {
                var toReturn = (double)left != (double)right;
                Evaluator.Diagnostics.RemoveError();
                return toReturn;
            }
            catch (Exception e)
            {
                Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
            }
        }
        else if (Operator.Type == TokenType.Greater)
        {
            try
            {
                return (double)left > (double)right;
            }
            catch (Exception e)
            {
                Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
            }
        }
        else if (Operator.Type == TokenType.Less)
        {
            try
            {
                return (double)left < (double)right;
            }
            catch (Exception e)
            {
                Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
            }
        }
        else if (Operator.Type == TokenType.GreaterOrEqual)
        {
            try
            {
                return (double)left >= (double)right;
            }
            catch (Exception e)
            {
                Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
            }
        }
        else if (Operator.Type == TokenType.LessOrEqual)
        {
            try
            {
                return (double)left <= (double)right;
            }
            catch (Exception e)
            {
                Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
            }
        }
        else if (Operator.Type == TokenType.Or)
        {
            try
            {
                return (bool)left || (bool)right;
            }
            catch (Exception e)
            {
                Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
            }
        }
        else if (Operator.Type == TokenType.And)
        {
            try
            {
                return (bool)left && (bool)right;
            }
            catch (Exception e)
            {
                Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: {e.Message}");
            }
        }

        return null;
    }
}