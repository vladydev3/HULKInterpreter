namespace hulk;

class Evaluator
{
    private readonly Expression root;
    public List<string> Diagnostics = new();

    public Evaluator(Expression root)
    {
        this.root = root;
    }

    public object Evaluate()
    {
        return EvaluateExpression(root);
    }

    private object EvaluateExpression(Expression node)
    {
        if (node is StringExpression s)
        {
            s.StringToken.Text = s.StringToken.Text.Substring(1, s.StringToken.Text.Length - 2);
            s.StringToken.Text = s.StringToken.Text.Replace("\\t", "\t");
            s.StringToken.Text = s.StringToken.Text.Replace("\\n", "\n");
            s.StringToken.Text = s.StringToken.Text.Replace("\\", "");

            return s.StringToken.Text;
        }

        if (node is NumberExpression n) 
        {
            try
            {
                return (double)n.NumberToken.Value;
            }
            catch (Exception e)
            {
                Diagnostics.Add($"Semantic Error: {e.Message}");
            }
        }

        if (node is BooleanExpression boolean) 
        {
            try
            {
                return boolean.Bool.Value;
            }
            catch (System.Exception e)
            {
                Diagnostics.Add($"Semantic Error: {e.Message}");
            }
        }

        if (node is MathExpression t)
        {
            switch (t.MathFunc.Text)
            {
                case "sin":
                    try
                    {
                        return Math.Sin((double)EvaluateExpression(t.ExpressionInside));
                    }
                    catch (Exception e)
                    {
                        Diagnostics.Add($"Semantic Error: {e.Message}");
                    }
                    break;

                case "cos":
                    try
                    {
                        return Math.Cos((double)EvaluateExpression(t.ExpressionInside));
                    }
                    catch (Exception e)
                    {
                        Diagnostics.Add($"Semantic Error: {e.Message}");
                    }
                    break;
                case "sqrt":
                    try
                    {
                        return Math.Sqrt((double)EvaluateExpression(t.ExpressionInside));
                    }
                    catch (Exception e)
                    {
                        Diagnostics.Add($"Semantic Error: {e.Message}");
                    }
                    break;
                case "exp":
                    try
                    {
                        return Math.Pow(Math.E, (double)EvaluateExpression(t.ExpressionInside));
                    }
                    catch (Exception e)
                    {
                        Diagnostics.Add($"Semantic Error: {e.Message}");
                    }
                    break;
            }
        }

        if (node is LogExpression l)
        {
            try
            {
                return Math.Log((double)l.Number.Value, (double)l.Base.Value);
            }
            catch (Exception e)
            {
                Diagnostics.Add($"Semantic Error: {e.Message}");
            }
        }
        if (node is UnaryExpression u)
        {
            var operand = EvaluateExpression(u.Operand);

            if (u.OperatorToken.Type == TokenType.Negation)
            {
                try
                {
                    return !(bool)operand;
                }
                catch (Exception e)
                {
                    Diagnostics.Add($"Semantic Error: {e.Message}");
                }
            }
            else if (u.OperatorToken.Type == TokenType.Minus)
            {
                try
                {
                    return -(double)operand;
                }
                catch (Exception e)
                {
                    Diagnostics.Add($"Semantic Error: {e.Message}");
                }
            }
            else if (u.OperatorToken.Type == TokenType.Plus) return operand;
            else throw new Exception($"Unexpected unary operator {u.OperatorToken.Type}");
        }

        if (node is PrintExpression print) return EvaluateExpression(print.ExpressionInside);

        if (node is BinaryExpression b)
        {
            object? left = EvaluateExpression(b.Left);
            object? right = EvaluateExpression(b.Right);

            if (b.Operator.Type == TokenType.Plus)
            {
                try
                {
                    return (double)left + (double)right;
                }
                catch (Exception e)
                {
                    Diagnostics.Add($"Semantic Error: {e.Message}");
                }
            }
            else if (b.Operator.Type == TokenType.Minus)
            {
                try
                {
                    return (double)left - (double)right;
                }
                catch (Exception e)
                {
                    Diagnostics.Add($"Semantic Error: {e.Message}");
                }
            }
            else if (b.Operator.Type == TokenType.Mult)
            {
                try
                {
                    return (double)left * (double)right;
                }
                catch (Exception e)
                {
                    Diagnostics.Add($"Semantic Error: {e.Message}");
                }
            }
            else if (b.Operator.Type == TokenType.Div)
            {
                try
                {
                    return (double)left / (double)right;
                }
                catch (Exception e)
                {
                    Diagnostics.Add($"Semantic Error: {e.Message}");
                }
            }
            else if (b.Operator.Type == TokenType.Mod)
            {
                try
                {
                    return (double)left % (double)right;
                }
                catch (Exception e)
                {
                    Diagnostics.Add($"Semantic Error: {e.Message}");
                }
            }
            else if (b.Operator.Type == TokenType.Concat)
            {
                try
                {
                    return (string)left + right;
                }
                catch (Exception e)
                {
                    Diagnostics.Add($"Semantic Error: At least one of the elements to concatenate must be a string");
                }
                try
                {
                    return left + (string)right;
                }
                catch (Exception e)
                {
                    Diagnostics.Add($"Semantic Error: At least one of the elements to concatenate must be a string");
                }
            }
            else if (b.Operator.Type == TokenType.Pow)
            {
                try
                {
                    return Math.Pow((double)left, (double)right);
                }
                catch (Exception e)
                {
                    Diagnostics.Add($"Semantic Error: {e.Message}");
                }
            }
            else if (b.Operator.Type == TokenType.Comparation)
            {
                try
                {
                    return (double)left == (double)right;
                }
                catch (Exception e)
                {
                    Diagnostics.Add($"Semantic Error: {$"Semantic Error: {e.Message}"}");
                }

                try
                {
                    return (bool)left == (bool)right;
                }
                catch (Exception e)
                {
                    Diagnostics.Add($"Semantic Error: {$"Semantic Error: {e.Message}"}");
                }
            }
            else if (b.Operator.Type == TokenType.Bigger)
            {
                try
                {
                    return (double)left > (double)right;
                }
                catch (Exception e)
                {
                    Diagnostics.Add($"Semantic Error: {e.Message}");
                }
            }
            else if (b.Operator.Type == TokenType.Minor)
            {
                try
                {
                    return (double)left < (double)right;
                }
                catch (Exception e)
                {
                    Diagnostics.Add($"Semantic Error: {e.Message}");
                }
            }
            else if (b.Operator.Type == TokenType.BiggerOrEqual)
            {
                try
                {
                    return (double)left >= (double)right;
                }
                catch (Exception e)
                {
                    Diagnostics.Add($"Semantic Error: {e.Message}");
                }
            }
            else if (b.Operator.Type == TokenType.MinorOrEqual)
            {
                try
                {
                    return (double)left <= (double)right;
                }
                catch (Exception e)
                {
                    Diagnostics.Add($"Semantic Error: {e.Message}");
                }
            }
            else throw new Exception($"Unexpected binary operator {b.Operator.Type}");
        }

        if (node is ParenExpression p) return EvaluateExpression(p.Expression);

        return "";
    }
}