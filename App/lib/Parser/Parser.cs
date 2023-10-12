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
            case TokenType.Greater:
            case TokenType.GreaterOrEqual:
            case TokenType.Less:
            case TokenType.LessOrEqual:
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
    private int count = 0;

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

        if (Type == TokenType.RParen) Diagnostics.AddError($"! SYNTAX ERROR: Missing closing parenthesis after '{tokens[position - 1].Text}' (column {Current.Position+1}).");
        else if (Type == TokenType.EOL) Diagnostics.AddError($"! SYNTAX ERROR: ';' expected ");
        else Diagnostics.AddError($"! SYNTAX ERROR: Invalid token '{Current.Text}', expected '{Type}' (column {Current.Position+1}).");

        return new Token(Type, Current.Position, null, null);
    }
    private Token Match(string Type)
    {
        if (Current.Text == Type) return NextToken();

        Diagnostics.AddError($"! SYNTAX ERROR: Invalid token '{Current.Text}', expected '{Type}' (column {Current.Position+1}).");

        return new Token(TokenType.Error, Current.Position, null, null);
    }

    public SyntaxTree Parse()
    {
        var expression = ParseExpression();
        var endOfFileToken = Match(TokenType.EOL);
        return new SyntaxTree(Diagnostics, expression, endOfFileToken);
    }
    public Expression ParseExpression(int parentPrecedence = 0)
    {
        if (count > 1000)
        {
            Diagnostics.AddError($"! SYNTAX ERROR: Can't be parsed '{Current.Text}' (column {Current.Position+1}).");
            return null;
        }
        count++;
        Expression left;
        var unaryOperatorPrecedence = Current.Type.GetUnaryOperatorPrecedence();
        if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
        {
            var operatorToken = NextToken();
            var operand = ParseExpression(unaryOperatorPrecedence);
            if (operand == null)
            {
                Diagnostics.AddError($"! SYNTAX ERROR: Missing operand after '{operatorToken.Text}' operator (column {Current.Position+1}).");
            }
            left = new UnaryExpression(operatorToken, operand);
        }
        else left = ParsePrimaryExpression();

        while (true)
        {
            var precedence = Current.Type.GetBinaryOperatorPrecedence();
            if (precedence == 0 || precedence <= parentPrecedence) break;

            var operatorToken = NextToken();
            var right = ParseExpression(precedence);
            if (left == null || right == null)
            {
                Diagnostics.AddError($"! SYNTAX ERROR: Invalid expression '{tokens[position-1].Text}' (column {Current.Position})");
            }
            left = new BinaryExpression(left, operatorToken, right);
        }
        return left;
    }

    private Expression? ParsePrimaryExpression()
    {
        if (count > 1000)
        {
            Diagnostics.AddError($"! SYNTAX ERROR: Can't be parsed '{Current.Text}' (column {Current.Position+1}).");
            return null;
        }
        if (Current.Type == TokenType.LBracket)
        {
            Match(TokenType.LBracket);
            List<Expression> elements = new();
            if (Current.Type != TokenType.RBracket) elements.Add(ParseExpression());
            while (Current.Type != TokenType.RBracket)
            {
                Match(TokenType.Comma);
                elements.Add(ParseExpression());
            }
            Match(TokenType.RBracket);

            return new VectorExpression(elements);
        }

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
            var numberToken = Match(TokenType.Number);
            return new NumberExpression(numberToken);
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
                Match(TokenType.LParen);
                var bas = Match(TokenType.Number);
                Match(TokenType.Comma);
                var number = Match(TokenType.Number);
                Match(TokenType.RParen);

                return new LogExpression(logToken, bas, number);
            }
            if (Current.Text == "range")
            {
                NextToken();
                Match(TokenType.LParen);
                var start = ParseExpression();
                if (Current.Type == TokenType.Comma)
                {
                    NextToken();
                    var end = ParseExpression();
                    Match(TokenType.RParen);
                    return new VectorExpression(new RangeFunction(start, end).GetVector());
                }
                Match(TokenType.RParen);
                return new VectorExpression(new RangeFunction(new NumberExpression(new Token(TokenType.Number, 0, "0", 0)), start).GetVector());
            }
            if (Current.Text == "PI")
            {
                NextToken();

                return new NumberExpression(new Token(TokenType.Number, 0, Math.PI.ToString(), Math.PI));
            }
            if (Current.Text == "E")
            {
                NextToken();

                return new NumberExpression(new Token(TokenType.Number, 0, Math.E.ToString(), Math.E));
            }

            var trigToken = NextToken();
            Match(TokenType.LParen);
            var expression = ParseExpression();
            Match(TokenType.RParen);

            return new MathExpression(trigToken, expression);
        }

        if (Current.Type == TokenType.Keyword)
        {

            if (Current.Text == "print")
            {
                NextToken();
                Match(TokenType.LParen);
                var expression = ParseExpression();
                Match(TokenType.RParen);

                return new PrintExpression(expression);
            }
            if (Current.Text == "let")
            {
                List<Token> variablesNames = new();
                List<Expression> variablesExpressions = new();
                NextToken();
                variablesNames.Add(Match(TokenType.Identificator));
                Match(TokenType.Asignation);
                variablesExpressions.Add(ParseExpression());
                if (variablesExpressions[0] == null) Diagnostics.AddError($"! SYNTAX ERROR: Missing expression in 'let-in' after variable '{variablesNames[0].Text} (column {Current.Position+1})'");
                Evaluator.VariableScope.Add(new Tuple<string, Expression, int>(variablesNames[0].Text, variablesExpressions[0], ++Evaluator.ScopePointer));
                if (Current.Type == TokenType.Comma)
                {
                    do
                    {
                        NextToken();
                        variablesNames.Add(Match(TokenType.Identificator));
                        Match(TokenType.Asignation); ;
                        variablesExpressions.Add(ParseExpression());
                        Evaluator.VariableScope.Add(new Tuple<string, Expression, int>(variablesNames[variablesNames.Count - 1].Text, variablesExpressions[variablesExpressions.Count - 1], Evaluator.ScopePointer));
                    }
                    while (Current.Type == TokenType.Comma);
                    var inToken = Match("in");
                    if (inToken.Type == TokenType.Error)
                    {
                        Diagnostics.RemoveError();
                        Diagnostics.AddError($"! SYNTAX ERROR: Missing 'in' after 'let' expression (column {Current.Position+1}).");
                    }
                    var inexpression = ParseExpression();

                    if (inexpression == null)
                    {
                        Diagnostics.AddError($"! SYNTAX ERROR: Missing expression after 'in' (column {Current.Position+1}).");
                    }
                    return new LetInExpression(variablesNames, variablesExpressions, inexpression);
                }
                else
                {
                    Match("in");
                    var inExpression = ParseExpression();
                    if (inExpression == null)
                    {
                        Diagnostics.AddError($"! SYNTAX ERROR: Missing expression after 'in' (column {Current.Position+1}).");
                    }
                    return new LetInExpression(variablesNames, variablesExpressions, inExpression);
                }
            }
            if (Current.Text == "for")
            {
                NextToken();
                Match(TokenType.LParen);
                var identifier = Match(TokenType.Identificator);
                Match("in");
                if (Current.Type == TokenType.Identificator)
                {
                    var name = Match(TokenType.Identificator);
                    Match(TokenType.RParen);
                    var body = ParseExpression();

                    return new ForExpression(identifier, null, name, body);
                }
                else
                {
                    Match("range");
                    Match(TokenType.LParen);
                    var start = ParseExpression();
                    if (Current.Type == TokenType.Comma)
                    {
                        Match(TokenType.Comma);
                        var end = ParseExpression();
                        Match(TokenType.RParen);
                        Match(TokenType.RParen);
                        var body = ParseExpression();
                        return new ForExpression(identifier, new RangeFunction(start, end), null, body);
                    }
                    Match(TokenType.RParen);
                    Match(TokenType.RParen);
                    var body1 = ParseExpression();
                    return new ForExpression(identifier, new RangeFunction(new NumberExpression(new Token(TokenType.Number, 0, "0", 0)), start), null, body1);
                }

            }
            if (Current.Text == "if")
            {
                List<Expression> elifs = new();
                List<Expression> elifcondition = new();
                NextToken();
                Match(TokenType.LParen);
                var condition = ParseExpression();
                if (condition == null) Diagnostics.AddError($"! SYNTAX ERROR: Missing 'if' condition (column {Current.Position+1}).");
                Match(TokenType.RParen);
                var ifexpression = ParseExpression();
                while (Current.Text == "elif")
                {
                    NextToken();
                    Match(TokenType.LParen);
                    elifcondition.Add(ParseExpression());
                    Match(TokenType.RParen);
                    elifs.Add(ParseExpression());
                }
                Match("else");
                var elseexpression = ParseExpression();

                return new IfExpression(condition, ifexpression, elifcondition, elifs, elseexpression);
            }
            if (Current.Text == "function")
            {
                ParseFunctionExpression();
                return null;
            }
            if (Current.Text == "while")
            {
                NextToken();
                Match(TokenType.LParen);
                var condition = ParseExpression();
                Match(TokenType.RParen);
                var body = ParseExpression();

                return new WhileExpression(condition, body);
            }
        }

        if (Current.Type == TokenType.Identificator)
        {
            var name = Match(TokenType.Identificator);

            if (Current.Type == TokenType.LParen)
            {
                Match(TokenType.LParen);
                List<Expression> arguments = new();
                if (Current.Type != TokenType.RParen) arguments.Add(ParseExpression());
                while (Current.Type != TokenType.RParen && Current.Type != TokenType.EOL)
                {
                    Match(TokenType.Comma);
                    if (Diagnostics.AnyError())
                    {
                        Diagnostics.RemoveError();
                        Diagnostics.AddError($"! SYNTAX ERROR: Missing closing parenthesis in '{name.Text}' call (column {Current.Position+1}).");
                        return null;
                    }
                    arguments.Add(ParseExpression());
                }
                Match(TokenType.RParen);
                return new FunctionCallExpression(name, arguments);
            }

            if (Current.Type == TokenType.LBracket)
            {
                Match(TokenType.LBracket);
                var index = ParseExpression();
                Match(TokenType.RBracket);

                return new NumberExpression(new Token(TokenType.Number, 0, VectorExpression.GetElement(name, int.Parse(Evaluator.Evaluate(index).ToString())).ToString(), VectorExpression.GetElement(name, int.Parse(Evaluator.Evaluate(index).ToString()))));
            }

            if (Current.Text == "current")
            {
                NextToken();
                Match(TokenType.LParen);
                Match(TokenType.RParen);

                return new CurrentFunction(name);
            }

            if (Current.Text == "next")
            {
                NextToken();
                Match(TokenType.LParen);
                Match(TokenType.RParen);

                return new NextFunction(name);
            }

            if (Current.Text == "size")
            {
                NextToken();
                Match(TokenType.LParen);
                Match(TokenType.RParen);

                return new NumberExpression(new Token(TokenType.Number, 0, VectorExpression.GetVector(name).Elements.Count.ToString(), VectorExpression.GetVector(name).Elements.Count));
            }

            return new VariableExpression(name, Evaluator.ScopePointer);
        }


        return null;
    }
    private void ParseFunctionExpression()
    {
        Match("function");
        var name = Match(TokenType.Identificator);
        Match(TokenType.LParen);
        List<Token> arguments = new();
        if (Current.Type != TokenType.RParen) arguments.Add(Match(TokenType.Identificator));
        while (Current.Type != TokenType.RParen)
        {
            Match(TokenType.Comma);
            arguments.Add(Match(TokenType.Identificator));
        }
        Match(TokenType.RParen);
        Match(TokenType.Asignation);
        Match(TokenType.Greater);
        var body = ParseExpression();

        foreach (var item in Evaluator.FunctionsScope)
        {
            if (item.Item1 == name.Text) Diagnostics.AddError($"! PARSER ERROR: \"{name.Text}\" is already defined.");
        }

        if (Diagnostics.AnyError()) return;
        Evaluator.FunctionsScope.Add(new Tuple<string, List<Token>, Expression>(name.Text, arguments, body));
    }
}



