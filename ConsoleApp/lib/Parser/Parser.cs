namespace Backend;

public class RecursiveParser
{
    List<Token> tokens = new List<Token>();
    int position = 0;

    public bool Parse(List<Token> code)
    {
        tokens = code;

        if (tokens[tokens.Count - 1].Type != Token.TokenType.EndLine) throw new Exception("Expected ';'");

        return E();
    }

    bool Match(Token.TokenType tokenType, string value = "")
    {
        if (value != "") return tokens[position++].Value == value;
        return tokens[position++].Type == tokenType;
    }

    bool E()
    {
        int currentToken = position;
        if (E1()) return true;

        position = currentToken;
        if (E2()) return true;

        position = currentToken;
        if (E3()) return true;

        position = currentToken;
        if (E4()) return true;

        return false;
    }

    bool E1()
    {
        // E -> T +- E
        return T() && Match(Token.TokenType.PlusMinus) && E();
    }

    bool E2()
    {
        // PrintExp -> "print"(E)
        return Match(Token.TokenType.Identificator, "print") && F2();
    }

    bool E3()
    {
        // E -> T
        return T();
    }

    bool E4()
    {
        // VarExp -> let id = value in E
        return Match(Token.TokenType.Keyword, "let") && Match(Token.TokenType.Identificator) && Match(Token.TokenType.Asignation) && (Match(Token.TokenType.Number)) && Match(Token.TokenType.Keyword, "in") && E();
    }

    bool T()
    {
        int currentToken = position;
        if (T1()) return true;

        position = currentToken;
        if (T2()) return true;

        return false;
    }

    bool T1()
    {
        // T -> F */ T
        return F() && (Match(Token.TokenType.MultDiv)) && T();
    }

    bool T2()
    {
        // T -> F
        return F();
    }

    bool F()
    {
        int currentToken = position;
        if (F1()) return true;

        position = currentToken;
        if (F2()) return true;

        return false;
    }

    bool F1()
    {
        // F -> D
        return D();
    }

    bool F2()
    {
        // F -> (E)
        return Match(Token.TokenType.LParen) && E() && Match(Token.TokenType.RParen);
    }

    bool D()
    {
        int currentToken = position;
        if (D1()) return true;

        position = currentToken;
        if (D2()) return true;

        return false;
    }

    bool D1()
    {
        // D -> id
        return Match(Token.TokenType.Identificator);
    }

    bool D2()
    {
        // D -> number
        return Match(Token.TokenType.Number);
    }

}
