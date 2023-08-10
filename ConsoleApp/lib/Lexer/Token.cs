namespace Backend;

public class Token
{
    public enum TokenType
    {
        Keyword,
        Number,
        PlusMinus,
        MultDiv,
        Asignation,
        String,
        Boolean,
        Identificator,
        LParen,
        RParen,
        EndLine
    }

    public TokenType Type { get; protected set; }
    public string Value { get; protected set; }

    public Token(TokenType type, string value)
    {
        Type = type;
        Value = value;
    }
}



/*

*/