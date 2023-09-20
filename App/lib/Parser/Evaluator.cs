using System.Linq.Expressions;

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
        if (node is NumberExpression n) return (int)n.NumberToken.Value;

        if (node is UnaryExpression u){
            var operand = EvaluateExpression(u.Operand);

            if (u.OperatorToken.Type == TokenType.Plus) return operand;
            else if (u.OperatorToken.Type == TokenType.Minus) return -(int)operand;
            else throw new Exception($"Unexpected unary operator {u.OperatorToken.Type}");
        }

        if (node is PrintExpression print) return EvaluateExpression(print.ExpressionInside);

        if (node is BinaryExpression b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);

            if (b.Operator.Type == TokenType.Plus) return (int)left + (int)right;
            else if (b.Operator.Type == TokenType.Minus) return (int)left - (int)right;
            else if (b.Operator.Type == TokenType.Mult) return (int)left * (int)right;
            else if (b.Operator.Type == TokenType.Div) return (int)left / (int)right;
            else if (b.Operator.Type == TokenType.Div) return (int)left % (int)right;
            else throw new Exception($"Unexpected binary operator {b.Operator.Type}");
        }

        if (node is ParenExpression p) return EvaluateExpression(p.Expression);

        return "";

        //throw new Exception($"ERROR: Unexpected node");
    }
}