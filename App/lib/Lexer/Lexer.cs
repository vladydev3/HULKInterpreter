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
                string number = string.Empty;
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
                    diagnostics.AddError($"! LEXICAL ERROR: '{number + currentChar}' isn't a valid token (column {currentIndex + 1})");
                }
                tokens.Add(new Token(TokenType.Number, currentIndex, number, double.Parse(number)));
                continue;
            }
            else if (currentChar == '.')
            {
                currentChar = code[++currentIndex];
                string funct = string.Empty;
                while (char.IsLetter(currentChar))
                {
                    funct += currentChar;
                    currentIndex++;
                    if (currentIndex >= code.Length) break;
                    currentChar = code[currentIndex];
                }
                if (funct == "next" || funct == "current" || funct == "size")
                {
                    tokens.Add(new Token(TokenType.UtilFunctions, currentIndex, funct, string.Empty));
                    continue;
                }
                if (funct == string.Empty) diagnostics.AddError($"! LEXICAL ERROR: '{currentChar}' isn't a valid token (column {currentIndex + 1})");
                else diagnostics.AddError($"! LEXICAL ERROR: '{funct}' isn't a valid token (column {currentIndex + 1})");
                continue;
            }
            else if (char.IsLetter(currentChar) || currentChar == '_')
            {
                string identifier = string.Empty;
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
                    tokens.Add(new Token(TokenType.Keyword, currentIndex, identifier, string.Empty));
                    continue;
                }
                else if (identifier == "elif")
                {
                    tokens.Add(new Token(TokenType.Keyword, currentIndex, identifier, string.Empty));
                    continue;
                }
                else if (identifier == "for")
                {
                    tokens.Add(new Token(TokenType.Keyword, currentIndex, identifier, string.Empty));
                    continue;
                }
                else if (identifier == "while")
                {
                    tokens.Add(new Token(TokenType.Keyword, currentIndex, identifier, string.Empty));
                    continue;
                }
                else if (identifier == "if")
                {
                    tokens.Add(new Token(TokenType.Keyword, currentIndex, identifier, string.Empty));
                    continue;
                }
                else if (identifier == "let")
                {
                    tokens.Add(new Token(TokenType.Keyword, currentIndex, identifier, string.Empty));
                    continue;
                }
                else if (identifier == "in")
                {
                    tokens.Add(new Token(TokenType.Keyword, currentIndex, identifier, string.Empty));
                    continue;
                }
                else if (identifier == "function")
                {
                    tokens.Add(new Token(TokenType.Keyword, currentIndex, identifier, string.Empty));
                    continue;
                }
                else if (identifier == "print")
                {
                    tokens.Add(new Token(TokenType.Keyword, currentIndex, identifier, string.Empty));
                    continue;
                }
                else if (identifier == "PI" || identifier == "sin" || identifier == "cos" || identifier == "log" || identifier == "exp" || identifier == "rand" || identifier == "sqrt" || identifier == "E" || identifier == "range")
                {
                    tokens.Add(new Token(TokenType.MathFunctions, currentIndex, identifier, string.Empty));
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
                tokens.Add(new Token(TokenType.Plus, currentIndex, "+", string.Empty));
                currentIndex++;
                continue;
            }
            else if (currentChar == '-')
            {
                tokens.Add(new Token(TokenType.Minus, currentIndex, "-", string.Empty));
                currentIndex++;
                continue;
            }
            else if (currentChar == '*')
            {
                tokens.Add(new Token(TokenType.Mult, currentIndex, "*", string.Empty));
                currentIndex++;
                continue;
            }
            else if (currentChar == '/')
            {
                tokens.Add(new Token(TokenType.Div, currentIndex, "/", string.Empty));
                currentIndex++;
                continue;
            }
            else if (currentChar == '(')
            {
                tokens.Add(new Token(TokenType.LParen, currentIndex, "(", string.Empty));
                currentIndex++;
                continue;
            }
            else if (currentChar == ')')
            {
                tokens.Add(new Token(TokenType.RParen, currentIndex, ")", string.Empty));
                currentIndex++;
                continue;
            }
            else if (currentChar == '=')
            {
                currentIndex++;
                if (code[currentIndex] == '=')
                {
                    tokens.Add(new Token(TokenType.Comparation, currentIndex, "==", string.Empty));
                    currentIndex++;
                }
                else tokens.Add(new Token(TokenType.Asignation, currentIndex - 1, "=", string.Empty));
                continue;
            }
            else if (currentChar == '!')
            {
                currentIndex++;
                if (currentIndex >= code.Length)
                {
                    tokens.Add(new Token(TokenType.Negation, currentIndex - 1, "!", string.Empty));
                    continue;
                }
                if (code[currentIndex] == '=')
                {
                    tokens.Add(new Token(TokenType.Diferent, currentIndex, "!=", string.Empty));
                    currentIndex++;
                }
                else tokens.Add(new Token(TokenType.Negation, currentIndex - 1, "!", string.Empty));
                continue;
            }
            else if (currentChar == '<')
            {
                currentIndex++;
                if (code[currentIndex] == '=')
                {
                    tokens.Add(new Token(TokenType.LessOrEqual, currentIndex, "<=", string.Empty));
                    currentIndex++;
                }
                else tokens.Add(new Token(TokenType.Less, currentIndex - 1, "<", string.Empty));
                continue;
            }
            else if (currentChar == '>')
            {
                if (code[currentIndex] == '=')
                {
                    tokens.Add(new Token(TokenType.GreaterOrEqual, currentIndex, ">=", string.Empty));
                    currentIndex++;
                }
                else tokens.Add(new Token(TokenType.Greater, currentIndex - 1, ">", string.Empty));
                currentIndex++;
                continue;
            }
            else if (currentChar == '&')
            {
                tokens.Add(new Token(TokenType.And, currentIndex, "&", string.Empty));
                currentIndex++;
                continue;
            }
            else if (currentChar == '|')
            {
                tokens.Add(new Token(TokenType.Or, currentIndex, "|", string.Empty));
                currentIndex++;
                continue;
            }
            else if (currentChar == ',')
            {
                tokens.Add(new Token(TokenType.Comma, currentIndex, ",", string.Empty));
                currentIndex++;
                continue;
            }
            else if (currentChar == ';')
            {
                tokens.Add(new Token(TokenType.EOL, currentIndex, ";", string.Empty));
                currentIndex++;
                continue;
            }
            else if (currentChar == '[')
            {
                tokens.Add(new Token(TokenType.LBracket, currentIndex, "[", string.Empty));
                currentIndex++;
                continue;
            }
            else if (currentChar == ']')
            {
                tokens.Add(new Token(TokenType.RBracket, currentIndex, "]", string.Empty));
                currentIndex++;
                continue;
            }
            else if (currentChar == '^')
            {
                tokens.Add(new Token(TokenType.Pow, currentIndex, "^", string.Empty));
                currentIndex++;
                continue;
            }
            else if (currentChar == '%')
            {
                tokens.Add(new Token(TokenType.Mod, currentIndex, "%", string.Empty));
                currentIndex++;
                continue;
            }
            else if (currentChar == '@')
            {
                tokens.Add(new Token(TokenType.Concat, currentIndex, "@", string.Empty));
                currentIndex++;
                continue;
            }
            else if (currentChar == '"')
            {
                string str = string.Empty;
                currentIndex++;
                if (currentIndex >= code.Length)
                {
                    diagnostics.AddError($"! LEXICAL ERROR: Unterminated String '{currentChar}' (column {currentIndex + 1})");
                    break;
                }
                try
                {
                    while (code[currentIndex] != '"')
                    {
                        if (code[currentIndex] == '\\')
                        {
                            currentIndex++;
                            if (currentIndex >= code.Length)
                            {
                                diagnostics.AddError($"! LEXICAL ERROR: Unterminated String '{currentChar}' (column {currentIndex + 1})");
                                break;
                            }
                            if (code[currentIndex] == 'n')
                            {
                                str += '\n';
                                currentIndex++;
                            }
                            else if (code[currentIndex] == 't')
                            {
                                str += '\t';
                                currentIndex++;
                            }
                            else if (code[currentIndex] == '"')
                            {
                                str += '"';
                                currentIndex++;
                            }
                            else if (code[currentIndex] == '\\')
                            {
                                str += '\\';
                                currentIndex++;
                            }
                            else
                            {
                                diagnostics.AddError($"! LEXICAL ERROR: Invalid escape sequence '\\{code[currentIndex]}' (column {currentIndex + 1})");
                                currentIndex++;
                            }
                        }
                        else
                        {
                            str += code[currentIndex];
                            currentIndex++;
                        }
                    }


                }
                catch (Exception e)
                {
                    if (currentIndex >= code.Length) diagnostics.AddError($"! LEXICAL ERROR: Unterminated String '{str}' (column {currentIndex + 1})");
                    else diagnostics.AddError($"! LEXICAL ERROR: {e} (column {currentIndex + 1})");
                }
                tokens.Add(new Token(TokenType.String, currentIndex, str, str));
                currentIndex++;
                continue;
            }
            else if (currentChar == '[')
            {
                tokens.Add(new Token(TokenType.LBracket, currentIndex, "[", string.Empty));
                currentIndex++;
                continue;
            }
            else if (currentChar == ']')
            {
                tokens.Add(new Token(TokenType.RBracket, currentIndex, "]", string.Empty));
            }
            diagnostics.AddError($"! LEXICAL ERROR: '{currentChar}' isn't a valid token (column {currentIndex + 1})");

        }
        return tokens;
    }
}