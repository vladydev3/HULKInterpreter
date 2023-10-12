namespace hulk;

public class Lexer
{
    private readonly string code;
    public Errors diagnostics = new();
    public int currentIndex = 0;

    public Lexer(string code)
    {
        this.code = code;
    }
    public List<Token> Tokenize()
    {
        List<Token> tokens = new();

        while (currentIndex < code.Length)
        {
            char currentChar = code[currentIndex];
            if (char.IsDigit(currentChar))
            {
                string number = "";
                while (char.IsDigit(currentChar) || currentChar == '.')
                {
                    number += currentChar;
                    currentIndex++;
                    if (currentIndex >= code.Length)
                    {
                        break;
                    }
                    currentChar = code[currentIndex];
                }
                if (currentChar != ' ' && char.IsLetter(currentChar))
                {
                    diagnostics.AddError($"! LEXICAL ERROR: '{number + currentChar}' isn't a valid token (column {currentIndex})");
                }
                tokens.Add(new Token(TokenType.Number, currentIndex, number, double.Parse(number)));
                continue;
            }
            else if (currentChar == '.')
            {
                currentChar = code[++currentIndex];
                string funct = "";
                while (char.IsLetter(currentChar))
                {
                    funct += currentChar;
                    currentIndex++;
                    if (currentIndex >= code.Length) break;
                    currentChar = code[currentIndex];
                }
                if (funct == "next" || funct == "current" || funct == "size")
                {
                    tokens.Add(new Token(TokenType.UtilFunctions, currentIndex, funct, null));
                    continue;
                }
                if (funct == "") diagnostics.AddError($"! LEXICAL ERROR: '{currentChar}' isn't a valid token (column {currentIndex})");
                else diagnostics.AddError($"! LEXICAL ERROR: '{funct}' isn't a valid token (column {currentIndex})");
                continue;
            }
            else if (char.IsLetter(currentChar) || currentChar == '_')
            {
                string identifier = "";
                while (char.IsLetter(currentChar) || currentChar == '_' || char.IsDigit(currentChar))
                {
                    identifier += currentChar;
                    currentIndex++;
                    if (currentIndex >= code.Length) break;
                    currentChar = code[currentIndex];
                }

                if (identifier == "true")
                {
                    tokens.Add(new Token(TokenType.Boolean, currentIndex, identifier, true));
                    continue;
                }
                else if (identifier == "false")
                {
                    tokens.Add(new Token(TokenType.Boolean, currentIndex, identifier, false));
                    continue;
                }
                else if (identifier == "else")
                {
                    tokens.Add(new Token(TokenType.Keyword, currentIndex, identifier, null));
                    continue;
                }
                else if (identifier == "elif")
                {
                    tokens.Add(new Token(TokenType.Keyword, currentIndex, identifier, null));
                    continue;
                }
                else if (identifier == "for")
                {
                    tokens.Add(new Token(TokenType.Keyword, currentIndex, identifier, null));
                    continue;
                }
                else if (identifier == "while")
                {
                    tokens.Add(new Token(TokenType.Keyword, currentIndex, identifier, null));
                    continue;
                }
                else if (identifier == "if")
                {
                    tokens.Add(new Token(TokenType.Keyword, currentIndex, identifier, null));
                    continue;
                }
                else if (identifier == "let")
                {
                    tokens.Add(new Token(TokenType.Keyword, currentIndex, identifier, null));
                    continue;
                }
                else if (identifier == "in")
                {
                    tokens.Add(new Token(TokenType.Keyword, currentIndex, identifier, null));
                    continue;
                }
                else if (identifier == "function")
                {
                    tokens.Add(new Token(TokenType.Keyword, currentIndex, identifier, null));
                    continue;
                }
                else if (identifier == "print")
                {
                    tokens.Add(new Token(TokenType.Keyword, currentIndex, identifier, null));
                    continue;
                }
                else if (identifier == "PI" || identifier == "sin" || identifier == "cos" || identifier == "log" || identifier == "exp" || identifier == "rand" || identifier == "sqrt" || identifier == "E" || identifier == "range")
                {
                    tokens.Add(new Token(TokenType.MathFunctions, currentIndex, identifier, null));
                    continue;
                }
                tokens.Add(new Token(TokenType.Identificator, currentIndex, identifier, identifier));
                continue;
            }
            else if (currentChar == ' ')
            {
                currentIndex++;
                continue;
            }

            else if (currentChar == '+')
            {
                tokens.Add(new Token(TokenType.Plus, currentIndex, "+", null));
                currentIndex++;
                continue;
            }
            else if (currentChar == '-')
            {
                tokens.Add(new Token(TokenType.Minus, currentIndex, "-", null));
                currentIndex++;
                continue;
            }
            else if (currentChar == '*')
            {
                tokens.Add(new Token(TokenType.Mult, currentIndex, "*", null));
                currentIndex++;
                continue;
            }
            else if (currentChar == '/')
            {
                tokens.Add(new Token(TokenType.Div, currentIndex, "/", null));
                currentIndex++;
                continue;
            }
            else if (currentChar == '(')
            {
                tokens.Add(new Token(TokenType.LParen, currentIndex, "(", null));
                currentIndex++;
                continue;
            }
            else if (currentChar == ')')
            {
                tokens.Add(new Token(TokenType.RParen, currentIndex, ")", null));
                currentIndex++;
                continue;
            }
            else if (currentChar == '=')
            {
                currentIndex++;
                if (code[currentIndex] == '=')
                {
                    tokens.Add(new Token(TokenType.Comparation, currentIndex, "==", null));
                    currentIndex++;
                }
                else tokens.Add(new Token(TokenType.Asignation, currentIndex - 1, "=", null));
                continue;
            }
            else if (currentChar == '!')
            {
                currentIndex++;
                if(currentIndex>=code.Length)
                {
                    tokens.Add(new Token(TokenType.Negation, currentIndex - 1, "!", null));
                    continue;
                }
                if (code[currentIndex] == '=')
                {
                    tokens.Add(new Token(TokenType.Diferent, currentIndex, "!=", null));
                    currentIndex++;
                }
                else tokens.Add(new Token(TokenType.Negation, currentIndex - 1, "!", null));
                continue;
            }
            else if (currentChar == '<')
            {
                currentIndex++;
                if (code[currentIndex] == '=')
                {
                    tokens.Add(new Token(TokenType.LessOrEqual, currentIndex, "<=", null));
                    currentIndex++;
                }
                else tokens.Add(new Token(TokenType.Less, currentIndex - 1, "<", null));
                continue;
            }
            else if (currentChar == '>')
            {
                if (code[currentIndex] == '=')
                {
                    tokens.Add(new Token(TokenType.GreaterOrEqual, currentIndex, ">=", null));
                    currentIndex++;
                }
                else tokens.Add(new Token(TokenType.Greater, currentIndex - 1, ">", null));
                currentIndex++;
                continue;
            }
            else if (currentChar == '&')
            {
                tokens.Add(new Token(TokenType.And, currentIndex, "&", null));
                currentIndex++;
                continue;
            }
            else if (currentChar == '|')
            {
                tokens.Add(new Token(TokenType.Or, currentIndex, "|", null));
                currentIndex++;
                continue;
            }
            else if (currentChar == ',')
            {
                tokens.Add(new Token(TokenType.Comma, currentIndex, ",", null));
                currentIndex++;
                continue;
            }
            else if (currentChar == ';')
            {
                tokens.Add(new Token(TokenType.EOL, currentIndex, ";", null));
                currentIndex++;
                continue;
            }
            else if (currentChar == '[')
            {
                tokens.Add(new Token(TokenType.LBracket, currentIndex, "[", null));
                currentIndex++;
                continue;
            }
            else if (currentChar == ']')
            {
                tokens.Add(new Token(TokenType.RBracket, currentIndex, "]", null));
                currentIndex++;
                continue;
            }
            else if (currentChar == '^')
            {
                tokens.Add(new Token(TokenType.Pow, currentIndex, "^", null));
                currentIndex++;
                continue;
            }
            else if (currentChar == '%')
            {
                tokens.Add(new Token(TokenType.Mod, currentIndex, "%", null));
                currentIndex++;
                continue;
            }
            else if (currentChar == '@')
            {
                tokens.Add(new Token(TokenType.Concat, currentIndex, "@", null));
                currentIndex++;
                continue;
            }
            else if (currentChar == '"')
            {
                string str = "";
                currentIndex++;
                if (currentIndex >= code.Length)
                {
                    diagnostics.AddError($"! LEXICAL ERROR: Unterminated String '{currentChar}' (column {currentIndex})");
                    break;
                }
                try
                {
                    while (code[currentIndex] != '"')
                    {
                        str += code[currentIndex];
                        currentIndex++;
                    }
                }
                catch (System.Exception e)
                {
                    if (currentIndex >= code.Length) diagnostics.AddError($"! LEXICAL ERROR: Unterminated String '{str}' (column {currentIndex})");
                    else diagnostics.AddError($"! LEXICAL ERROR: {e} (column {currentIndex})");
                }
                tokens.Add(new Token(TokenType.String, currentIndex, str, str));
                currentIndex++;
                continue;
            }
            else if (currentChar == '[')
            {
                tokens.Add(new Token(TokenType.LBracket, currentIndex, "[", null));
                currentIndex++;
                continue;
            }
            else if (currentChar == ']')
            {
                tokens.Add(new Token(TokenType.RBracket, currentIndex, "]", null));
            }
            diagnostics.AddError($"! LEXICAL ERROR: '{currentChar}' isn't a valid token (column {currentIndex})");

        }
        return tokens;
    }
}