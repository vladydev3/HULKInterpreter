using System.Text.RegularExpressions;

namespace Backend;

public class Lexer
{
    private List<Tuple<Regex, Token.TokenType>> regexToTokenType = new List<Tuple<Regex, Token.TokenType>> {
        Tuple.Create(new Regex(@"\("), Token.TokenType.LParen),
        Tuple.Create(new Regex(@"\)"), Token.TokenType.RParen),
        Tuple.Create(new Regex(@"\d+"), Token.TokenType.Number),
        Tuple.Create(new Regex("true|false"), Token.TokenType.Boolean),
        Tuple.Create(new Regex("="), Token.TokenType.Asignation),
        Tuple.Create(new Regex(@"\+|\-"), Token.TokenType.PlusMinus),
        Tuple.Create(new Regex(@"\*|\/"), Token.TokenType.MultDiv),
        Tuple.Create(new Regex("print"), Token.TokenType.Keyword),
        Tuple.Create(new Regex("let"), Token.TokenType.Keyword),
        Tuple.Create(new Regex("in"), Token.TokenType.Keyword),
        Tuple.Create(new Regex("\"(.*?)\""), Token.TokenType.String),
        Tuple.Create(new Regex(@"\b[a-zA-Z_]\w*\b"), Token.TokenType.Identificator),
        Tuple.Create(new Regex(";"), Token.TokenType.EndLine)
    };

    public List<Token> Tokenize(string code)
    {
        List<Token> tokens = new List<Token>();
        int currentIndex = 0;

        while (currentIndex < code.Length)
        {
            bool matchFound = false;

            foreach (var regexToken in regexToTokenType)
            {
                var match = regexToken.Item1.Match(code, currentIndex);
                if (match.Success && match.Index == currentIndex)
                {
                    tokens.Add(new Token(regexToken.Item2, match.Value));
                    currentIndex += match.Length;
                    matchFound = true;
                    break;
                }
            }

            if (currentIndex < code.Length && code[currentIndex] == ' ')
            {
                currentIndex++;
            }

            if (!matchFound) throw new Exception($"Unexpected token.");
        }

        return tokens;
    }
}