namespace hulk;
public enum InferenceType
{
    None,
    Bool,
    Number,
    String,
    Object,
    Any
}

public class InferenceTypes
{
    public static InferenceType GetInferenceType(object value)
    {
        if (value is bool) return InferenceType.Bool;
        if (value is double) return InferenceType.Number;
        if (value is string) return InferenceType.String;
        if (value is object) return InferenceType.Object;
        return InferenceType.None;
    }

    public static InferenceType GetInferenceType(Token token)
    {
        if (token.Type == TokenType.Boolean) return InferenceType.Bool;
        if (token.Type == TokenType.Number || token.Type == TokenType.BiggerOrEqual || token.Type == TokenType.Bigger || token.Type == TokenType.Minor || token.Type == TokenType.MinorOrEqual) return InferenceType.Number;
        if (token.Type == TokenType.String) return InferenceType.String;
        if (token.Type == TokenType.Plus || token.Type == TokenType.Minus || token.Type == TokenType.Mult || token.Type == TokenType.Div || token.Type == TokenType.Mod || token.Type == TokenType.Negation || token.Type == TokenType.Pow) return InferenceType.Number;
        if (token.Type == TokenType.Pow) return InferenceType.Number;
        if (token.Type == TokenType.And || token.Type == TokenType.Or || token.Type == TokenType.Diferent) return InferenceType.Bool;
        if (token.Type == TokenType.Identificator)
        {
            foreach (Tuple<string, Expression> item in Evaluator.VariableScope)
            {
                if (item.Item1 == token.Text)
                {
                    return GetInferenceType(item.Item2);
                }
            }
            return InferenceType.Any;
        }
        return InferenceType.None;
    }

    public static InferenceType GetInferenceType(Expression expression)
    {
        if (expression is StringExpression stringExpression) return GetInferenceType(stringExpression.StringToken);
        if (expression is BooleanExpression booleanExpression) return GetInferenceType(booleanExpression.Bool);
        if (expression is NumberExpression numberExpression) return GetInferenceType(numberExpression.NumberToken);
        if (expression is VariableExpression variableExpression) return GetInferenceType(variableExpression.VariableName);
        if (expression is UnaryExpression unaryExpression)
        {
            var operand = GetInferenceType(unaryExpression.Operand);
            if (operand == InferenceType.Bool) return InferenceType.Bool;
            if (operand == InferenceType.Number) return InferenceType.Number;
            return InferenceType.Any;
        }
        if (expression is BinaryExpression binaryExpression)
        {
            // var left = GetInferenceType(binaryExpression.Left);
            // var right = GetInferenceType(binaryExpression.Right);
            // if (left == right) return left;
            // if (left == InferenceType.Any) return right;
            // if (right == InferenceType.Any) return left;

            return GetInferenceType(binaryExpression.Operator);
        }
        if (expression is FunctionCallExpression functionCallExpression)
        {
            foreach (var item in Evaluator.FunctionsScope)
            {
                if (item.Item1 == functionCallExpression.Name.Text)
                {
                    return GetInferenceType(item.Item3);
                }
            }
            return InferenceType.Any;
        }
        if (expression is IfExpression ifExpression)
        {
            var condition = GetInferenceType(ifExpression.Condition);
            var ifexp = GetInferenceType(ifExpression.ifExpression);
            var elseexp = GetInferenceType(ifExpression.ElseExpression);
            if (ifexp == elseexp) return ifexp;
            if (ifexp == InferenceType.Any) return elseexp;
            return InferenceType.Any;
        }
        return InferenceType.None;
    }
}