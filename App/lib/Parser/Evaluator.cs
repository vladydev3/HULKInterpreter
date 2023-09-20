using System.Linq.Expressions;

namespace hulk;

class Evaluator
{
    private readonly Expression root;

    public Evaluator(Expression root)
    {
        this.root = root;
    }

    public int Evaluate()
    {
        return EvaluateExpression(root);
    }

    private int EvaluateExpression(Expression node)
    {
        if (node is NumberExpression n) return (int)n.NumberToken.Value;

        if (node is UnaryExpression u){
            var operand = EvaluateExpression(u.Operand);

            if (u.OperatorToken.Type == TokenType.Plus) return operand;
            else if (u.OperatorToken.Type == TokenType.Minus) return -operand;
            else throw new Exception($"Unexpected unary operator {u.OperatorToken.Type}");
        }

        if (node is BinaryExpression b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);

            if (b.Operator.Type == TokenType.Plus) return left + right;
            else if (b.Operator.Type == TokenType.Minus) return left - right;
            else if (b.Operator.Type == TokenType.Mult) return left * right;
            else if (b.Operator.Type == TokenType.Div) return left / right;
            else throw new Exception($"Unexpected binary operator {b.Operator.Type}");
        }

        if (node is ParenExpression p) return EvaluateExpression(p.Expression);

        throw new Exception($"Unexpected node {node.Type}");
    }
}