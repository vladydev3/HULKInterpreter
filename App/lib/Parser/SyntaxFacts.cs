namespace hulk;

static class SyntaxFacts
{
    public static int GetUnaryOperatorPrecedence(this TokenType kind)
    {
        return kind switch
        {
            TokenType.Plus or TokenType.Minus => 3,
            TokenType.Negation => 1,
            _ => 0,
        };
    }
    public static int GetBinaryOperatorPrecedence(this TokenType type)
    {
        return type switch
        {
            TokenType.Pow => 5,
            TokenType.Mult or TokenType.Div => 4,
            TokenType.Plus or TokenType.Concat or TokenType.Minus => 3,
            TokenType.Greater or TokenType.GreaterOrEqual or TokenType.Less or TokenType.LessOrEqual or TokenType.Mod or TokenType.Comparation or TokenType.Diferent => 2,
            TokenType.And or TokenType.Or => 1,
            _ => 0,
        };
    }
}
