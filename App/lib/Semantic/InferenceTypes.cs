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
        if (token.Type == TokenType.Number) return InferenceType.Number;
        if (token.Type == TokenType.String) return InferenceType.String;
        if (token.Type == TokenType.Identificator)
        {
            foreach (Tuple<string, Expression> item in Evaluator.VariableScope)
            {
                if (item.Item1 == token.Text)
                {
                    return GetInferenceType(item.Item2);
                }
            }
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
            var left = GetInferenceType(binaryExpression.Left);
            var right = GetInferenceType(binaryExpression.Right);
            if (left == right) return left;
            return InferenceType.Any;
        }
        return InferenceType.None;
    }
}