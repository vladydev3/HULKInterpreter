namespace hulk;

public enum TokenType
{
    Keyword,
    Number,
    Plus,
    Minus,
    Mult,
    Div,
    Mod,
    Pow,
    Concat,
    Negation,
    MathFunctions,
    Print,
    Else,
    Comparation,
    BiggerOrEqual,
    MinorOrEqual,
    Bigger,
    Minor,
    Asignation,
    String,
    Boolean,
    Identificator,
    LParen,
    RParen,
    Parenthesis,
    Comma,
    WhiteSpace,
    Error,
    // Expressions
    FunctionExpression,
    IfExpression,
    LetInExpression,
    PrintExpression,
    UnaryExpression,
    BinaryExpression,
    EOL
}

public class Token : Node
{
    public override TokenType Type { get; }
    public int Position { get; }
    public string Text { get; set; }
    public object Value { get; }

    public Token(TokenType type, int position, string text, object value)
    {
        Type = type;
        Position = position;
        Text = text;
        Value = value;
    }

    public override IEnumerable<Node> GetChildren()
    {
        return Enumerable.Empty<Node>();
    }
}
