using System.Text.RegularExpressions;

namespace hulk;

public class Lexer
{

    private string code;
    private List<string> diagnostics = new();

    private readonly List<Tuple<Regex, TokenType>> regexToTokenType = new()
    {
        Tuple.Create(new Regex(@"\("), TokenType.LParen),
        Tuple.Create(new Regex(@"\)"), TokenType.RParen),
        Tuple.Create(new Regex(@"(?:\d+(?:\.\d*)?|\.\d+)"), TokenType.Number),
        Tuple.Create(new Regex("true|false"), TokenType.Boolean),
        Tuple.Create(new Regex(">="), TokenType.BiggerOrEqual),
        Tuple.Create(new Regex("<="), TokenType.MinorOrEqual),
        Tuple.Create(new Regex("<"), TokenType.Minor),
        Tuple.Create(new Regex(">"), TokenType.Bigger),
        Tuple.Create(new Regex("=="), TokenType.Comparation),
        Tuple.Create(new Regex("="), TokenType.Asignation),
        Tuple.Create(new Regex(@"\+"), TokenType.Plus),
        Tuple.Create(new Regex(@"\-"), TokenType.Minus),
        Tuple.Create(new Regex(@"\*"), TokenType.Mult),
        Tuple.Create(new Regex(@"\/"), TokenType.Div),
        Tuple.Create(new Regex(@"\%"), TokenType.Mod),
        Tuple.Create(new Regex(@"\^"), TokenType.Pow),
        Tuple.Create(new Regex(@"\@"), TokenType.Concat),
        Tuple.Create(new Regex("PI|sin|cos|log|exp|rand|sqrt|E"), TokenType.MathFunctions),
        Tuple.Create(new Regex("print"), TokenType.Print),
        Tuple.Create(new Regex("let"), TokenType.Keyword),
        Tuple.Create(new Regex("in"), TokenType.Keyword),
        Tuple.Create(new Regex("\"([^\"\\\\]|\\\\.)*\""), TokenType.String),
        Tuple.Create(new Regex(@"\b[a-zA-Z_]\w*\b"), TokenType.Identificator),
        Tuple.Create(new Regex(","), TokenType.Comma),
        Tuple.Create(new Regex(" "), TokenType.WhiteSpace),
        Tuple.Create(new Regex("!"), TokenType.Negation),
        Tuple.Create(new Regex(";"), TokenType.EOL)
    };

    public Lexer(string code)
    {
        this.code = code;
    }

    public IEnumerable<string> Diagnostics => diagnostics;

    public List<Token> Tokenize()
    {
        List<Token> tokens = new();
        int currentIndex = 0;

        while (currentIndex < code.Length)
        {
            bool matchFound = false;
            int len = 0;

            foreach (var regexToken in regexToTokenType)
            {
                var match = regexToken.Item1.Match(code, currentIndex);
                if (match.Success && match.Index == currentIndex)
                {
                    if (!double.TryParse(match.Value, out double value) && regexToken.Item2 == TokenType.Number)
                    {
                        diagnostics.Add($"The number {match.Value} isn't valid");
                    }

                    if (regexToken.Item2 == TokenType.MathFunctions)
                    {
                        var random = new Random();
                        double randomNum = random.NextDouble();
                        switch (match.Value)
                        {
                            case "PI":
                                tokens.Add(new Token(TokenType.Number, match.Index, "PI", Math.PI));
                                break;
                            case "E":
                                tokens.Add(new Token(TokenType.Number, match.Index, "E", Math.E));
                                break;
                            case "rand":
                                tokens.Add(new Token(TokenType.Number, match.Index, "rand", randomNum));
                                break;
                            default:
                                tokens.Add(new Token(regexToken.Item2, match.Index, match.Value, match.Value));
                                break;
                        }
                    }
                    else if (regexToken.Item2 == TokenType.Boolean)
                    {
                        bool val;
                        if (match.Value=="true")
                        {
                            val = true;
                            tokens.Add(new Token(regexToken.Item2, match.Index, code.Substring(match.Index, match.Length), val));
                        }
                        else {
                            val = false;
                            tokens.Add(new Token(regexToken.Item2, match.Index, code.Substring(match.Index, match.Length), val));
                        }                        
                    }
                    else if (!(regexToken.Item2 == TokenType.WhiteSpace))
                    {
                        tokens.Add(new Token(regexToken.Item2, match.Index, match.Value, value));
                    }
                    len = match.Length;
                    currentIndex += len;
                    matchFound = true;
                    break;
                }
                len = match.Length;
            }

            if (currentIndex < code.Length && code[currentIndex] == ' ')
            {
                currentIndex++;
            }

            if (!matchFound)
            {
                diagnostics.Add($"Lexical Error: {code.Substring(currentIndex, len)} is not a valid token");

                tokens.Add(new Token(TokenType.Error, currentIndex++, null, null));
            }
        }

        if (tokens[tokens.Count - 1].Type != TokenType.EOL)
        {
            tokens.Add(new Token(TokenType.Error, currentIndex, null, null));
        }

        return tokens;
    }
}