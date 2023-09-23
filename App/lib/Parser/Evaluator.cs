namespace hulk;

class Evaluator
{
    private readonly Expression root;

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
        if (node is StringExpression s) return s.StringToken.Text.Substring(1,s.StringToken.Text.Length-2);

        if (node is NumberExpression n) return (double)n.NumberToken.Value;

        if (node is MathExpression t)
        {
            switch (t.MathFunc.Text)
            {
                case "sin":
                    return Math.Sin((double)EvaluateExpression(t.ExpressionInside));
                case "cos":
                    return Math.Cos((double)EvaluateExpression(t.ExpressionInside));
            }
        }

        if (node is LogExpression l) return Math.Log((double)l.Number.Value, (double)l.Base.Value);

        if (node is UnaryExpression u){
            var operand = EvaluateExpression(u.Operand);

            if (u.OperatorToken.Type == TokenType.Plus) return operand;
            else if (u.OperatorToken.Type == TokenType.Minus) return -(double)operand;
            else throw new Exception($"Unexpected unary operator {u.OperatorToken.Type}");
        }

        if (node is PrintExpression print) return EvaluateExpression(print.ExpressionInside);

        if (node is BinaryExpression b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);

            if (b.Operator.Type == TokenType.Plus) return (double)left + (double)right;
            else if (b.Operator.Type == TokenType.Minus) return (double)left - (double)right;
            else if (b.Operator.Type == TokenType.Mult) return (double)left * (double)right;
            else if (b.Operator.Type == TokenType.Div) return (double)left / (double)right;
            else if (b.Operator.Type == TokenType.Div) return (double)left % (double)right;
            else if (b.Operator.Type == TokenType.Pow) return Math.Pow((double)left,(double)right);
            else throw new Exception($"Unexpected binary operator {b.Operator.Type}");
        }

        if (node is ParenExpression p) return EvaluateExpression(p.Expression);

        return "";

        //throw new Exception($"ERROR: Unexpected node");
    }
}