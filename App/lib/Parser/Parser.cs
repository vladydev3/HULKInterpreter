using System.Reflection.Metadata;

namespace hulk;
static class SyntaxFacts
{
    public static int GetUnaryOperatorPrecedence(this TokenType kind)
    {
        switch (kind)
        {
            case TokenType.Plus:
            case TokenType.Minus:
                return 3;
            case TokenType.Negation:
                return 1;
            default:
                return 0;
        }
    }
    public static int GetBinaryOperatorPrecedence(this TokenType type)
    {
        switch (type)
        {
            case TokenType.Pow:
                return 5;
            case TokenType.Mult:
            case TokenType.Div:
                return 4;
            case TokenType.Plus:
            case TokenType.Concat:
            case TokenType.Minus:
                return 3;
            case TokenType.Bigger:
            case TokenType.BiggerOrEqual:
            case TokenType.Minor:
            case TokenType.MinorOrEqual:
            case TokenType.Mod:
            case TokenType.Comparation:
            case TokenType.Diferent:
                return 2;
            case TokenType.And:
            case TokenType.Or:
                return 1;
            default:
                return 0;
        }
    }
}
class Parser
{
    private readonly Token[] tokens;
    public Errors Diagnostics = new();
    private int position;

    public Parser(string text)
    {
        Lexer lexer = new(text);

        var tokens = lexer.Tokenize();

        this.tokens = tokens.ToArray();
        Diagnostics = lexer.diagnostics;
    }

    private Token Peek(int offset)
    {
        var index = position + offset;
        if (index >= tokens.Length) return tokens[tokens.Length - 1];
        return tokens[index];
    }

    private Token Current => Peek(0);

    private Token NextToken()
    {
        var current = Current;
        position++;
        return current;
    }

    private Token Match(TokenType Type)
    {
        if (Current.Type == Type) return NextToken();

        if (Type == TokenType.RParen) Diagnostics.AddError($"! SYNTAX ERROR: Missing closing parenthesis after '{tokens[position - 1].Text}'.");
        else if (Type == TokenType.EOL) Diagnostics.AddError($"! SYNTAX ERROR: ';' expected ");
        else Diagnostics.AddError($"! SYNTAX ERROR: Invalid token '{Current.Text}', expected <{Type}> type.");

        return new Token(Type, Current.Position, null, null);
    }

    public SyntaxTree Parse()
    {
        var expression = ParseExpression();
        var endOfFileToken = Match(TokenType.EOL);
        return new SyntaxTree(Diagnostics, expression, endOfFileToken);
    }
    public Expression ParseExpression(int parentPrecedence = 0)
    {

        Expression left;
        var unaryOperatorPrecedence = Current.Type.GetUnaryOperatorPrecedence();
        if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
        {
            var operatorToken = NextToken();
            var operand = ParseExpression(unaryOperatorPrecedence);
            left = new UnaryExpression(operatorToken, operand);
        }
        else left = ParsePrimaryExpression();

        while (true)
        {
            var precedence = Current.Type.GetBinaryOperatorPrecedence();
            if (precedence == 0 || precedence <= parentPrecedence) break;

            var operatorToken = NextToken();
            var right = ParseExpression(precedence);
            left = new BinaryExpression(left, operatorToken, right);
        }
        return left;
    }

    private Expression? ParsePrimaryExpression()
    {
        if (Current.Type == TokenType.LParen)
        {
            var left = NextToken();
            var expression = ParseExpression();
            var right = Match(TokenType.RParen);

            return new ParenExpression(left, expression, right);
        }

        if (Current.Type == TokenType.Boolean)
        {
            var BoolToken = Match(TokenType.Boolean);
            return new BooleanExpression(BoolToken);
        }

        if (Current.Type == TokenType.Number)
        {
            if (Current.Text == "rand")
            {
                var random = Match(TokenType.Number);
                Match(TokenType.LParen);
                Match(TokenType.RParen);

                return new NumberExpression(random);
            }
            var numberToken = Match(TokenType.Number);
            return new NumberExpression(numberToken);
        }

        if (Current.Type == TokenType.Print)
        {
            var print = NextToken();
            var lParen = Match(TokenType.LParen);
            var expression = ParseExpression();
            var rParen = Match(TokenType.RParen);

            return new PrintExpression(print, lParen, expression, rParen);
        }

        if (Current.Type == TokenType.String)
        {
            var stringToken = Match(TokenType.String);

            return new StringExpression(stringToken);
        }

        if (Current.Type == TokenType.MathFunctions)
        {
            if (Current.Text == "log")
            {
                var logToken = NextToken();
                var openParen = Match(TokenType.LParen);
                var bas = Match(TokenType.Number);
                var comma = Match(TokenType.Comma);
                var number = Match(TokenType.Number);
                var closeParen = Match(TokenType.RParen);

                return new LogExpression(logToken, bas, number);
            }
            var trigToken = NextToken();
            var lParen = Match(TokenType.LParen);
            var expression = ParseExpression();
            var rParen = Match(TokenType.RParen);

            return new MathExpression(trigToken, lParen, expression, rParen);
        }

        if (Current.Type == TokenType.Keyword)
        {
            if (Current.Text == "let")
            {
                List<Token> variablesNames = new();
                List<Expression> variablesExpressions = new();
                Match(TokenType.Keyword);
                variablesNames.Add(Match(TokenType.Identificator));
                Match(TokenType.Asignation);
                variablesExpressions.Add(ParseExpression());
                if (variablesNames[0] == null) Diagnostics.AddError($"! SYNTAX ERROR: Missing expression in 'let-in' after variable '{variablesNames[0].Text}'");
                Evaluator.VariableScope.Add(new Tuple<string, Expression>(variablesNames[0].Text, variablesExpressions[0]));
                if (Current.Type == TokenType.Comma)
                {
                    while (Current.Type == TokenType.Comma)
                    {
                        Match(TokenType.Comma);
                        variablesNames.Add(Match(TokenType.Identificator));
                        Match(TokenType.Asignation); ;
                        variablesExpressions.Add(ParseExpression());
                        Evaluator.VariableScope.Add(new Tuple<string, Expression>(variablesNames[variablesNames.Count - 1].Text, variablesExpressions[variablesExpressions.Count - 1]));
                    }
                    Match(TokenType.Keyword);
                    var inexpression = ParseExpression();

                    return new LetInExpression(variablesNames, variablesExpressions, inexpression);
                }
                else
                {
                    Match(TokenType.Keyword);
                    var inExpression = ParseExpression();
                    return new LetInExpression(variablesNames, variablesExpressions, inExpression);
                }
            }
            if (Current.Text == "for")
            {
                Match(TokenType.Keyword);
                Match(TokenType.LParen);
                var identifier = Match(TokenType.Identificator);
                Match(TokenType.Keyword);
                Match(TokenType.MathFunctions);
                Match(TokenType.LParen);
                var start = ParseExpression();
                Match(TokenType.Comma);
                var end = ParseExpression();
                Match(TokenType.RParen);
                Match(TokenType.RParen);
                var body = ParseExpression();

                return new ForExpression(identifier, new RangeExpression(start, end), body);
            }
            if (Current.Text == "if")
            {
                List<Expression> elifs = new();
                List<Expression> elifcondition = new();
                Match(TokenType.Keyword);
                Match(TokenType.LParen);
                var condition = ParseExpression();
                Match(TokenType.RParen);
                var ifexpression = ParseExpression();
                while (Current.Type == TokenType.Elif)
                {
                    Match(TokenType.Elif);
                    Match(TokenType.LParen);
                    elifcondition.Add(ParseExpression());
                    Match(TokenType.RParen);
                    elifs.Add(ParseExpression());
                }
                Match(TokenType.Else);
                var elseexpression = ParseExpression();

                return new IfExpression(condition, ifexpression, elifcondition, elifs, elseexpression);
            }
            if (Current.Text == "function")
            {
                ParseFunctionExpression();
                return null;
            }
        }

        if (Current.Type == TokenType.Identificator)
        {
            var name = Match(TokenType.Identificator);

            if (Current.Type == TokenType.LParen)
            {
                Match(TokenType.LParen);
                List<Expression> arguments = new();
                if (Current.Type != TokenType.RParen && Current.Type != TokenType.Error) arguments.Add(ParseExpression());
                while (Current.Type != TokenType.RParen && Current.Type != TokenType.Error)
                {
                    Match(TokenType.Comma);
                    arguments.Add(ParseExpression());
                }
                Match(TokenType.RParen);
                return new FunctionCallExpression(name, arguments);
            }

            return new VariableExpression(name);
        }


        return null;
    }
    private void ParseFunctionExpression()
    {
        Match(TokenType.Keyword);
        var name = Match(TokenType.Identificator);
        Match(TokenType.LParen);
        List<Token> arguments = new();
        if (Current.Type != TokenType.RParen) arguments.Add(Match(TokenType.Identificator));
        while (Current.Type != TokenType.RParen && Current.Type != TokenType.Error)
        {
            Match(TokenType.Comma);
            arguments.Add(Match(TokenType.Identificator));
        }
        Match(TokenType.RParen);
        Match(TokenType.Asignation);
        Match(TokenType.Bigger);
        var body = ParseExpression();

        foreach (var item in Evaluator.FunctionsScope)
        {
            if (item.Item1 == name.Text) Diagnostics.AddError($"Parser Error: \"{name.Text}\" is already defined.");
        }

        Evaluator.FunctionsScope.Add(new Tuple<string, List<Token>, Expression>(name.Text, arguments, body));
    }
}



