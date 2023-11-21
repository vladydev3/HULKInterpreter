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
        if (token.Type == TokenType.Number || token.Type == TokenType.GreaterOrEqual || token.Type == TokenType.Greater || token.Type == TokenType.Less || token.Type == TokenType.LessOrEqual) return InferenceType.Number;
        if (token.Type == TokenType.String) return InferenceType.String;
        if (token.Type == TokenType.Plus || token.Type == TokenType.Minus || token.Type == TokenType.Mult || token.Type == TokenType.Div || token.Type == TokenType.Mod || token.Type == TokenType.Negation || token.Type == TokenType.Pow) return InferenceType.Number;
        if (token.Type == TokenType.Pow) return InferenceType.Number;
        if (token.Type == TokenType.And || token.Type == TokenType.Or || token.Type == TokenType.Diferent || token.Type == TokenType.Comparation) return InferenceType.Bool;
        if (token.Type == TokenType.Identificator)
        {
            foreach (var item in Evaluator.VariableScope)
            {
                if (item.Item1 == token.Text)
                {
                    return GetInferenceType(item.Item2);
                }
            }
            return InferenceType.Any;
        }
        return InferenceType.Any;
    }

    public static InferenceType GetInferenceType(Expression expression)
    {
        return expression switch
        {
            StringExpression stringExpression => GetInferenceType(stringExpression.StringToken),
            BooleanExpression booleanExpression => GetInferenceType(booleanExpression.Bool),
            NumberExpression numberExpression => numberExpression.NumberToken == null ? InferenceType.Any : GetInferenceType(numberExpression.NumberToken),
            VariableExpression variableExpression => GetInferenceType(variableExpression.VariableName),
            UnaryExpression unaryExpression => GetInferenceType(unaryExpression.Operand),
            BinaryExpression binaryExpression => GetInferenceType(binaryExpression.Operator),
            FunctionCallExpression _ => InferenceType.Any,
            IfExpression ifExpression => HandleIfExpression(ifExpression),
            ForExpression forExpression => HandleLoopExpression(forExpression.Body),
            WhileExpression whileExpression => HandleLoopExpression(whileExpression.body),
            _ => InferenceType.None,
        };
    }

    private static InferenceType HandleIfExpression(IfExpression ifExpression)
    {
        var condition = GetInferenceType(ifExpression.Condition);
        var ifexp = GetInferenceType(ifExpression.ifExpression);
        var elseexp = GetInferenceType(ifExpression.ElseExpression);
        if (condition == InferenceType.Any)
        {
            Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Condition must be a boolean expression.");
            return InferenceType.None;
        }
        if (ifexp == elseexp) return ifexp;
        return InferenceType.Any;
    }

    private static InferenceType HandleLoopExpression(Expression loopBody)
    {
        var loopExp = GetInferenceType(loopBody);
        if (loopExp == InferenceType.Any) return loopExp;
        return InferenceType.Any;
    }
}