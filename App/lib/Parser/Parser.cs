using System.Reflection.Metadata;

namespace hulk;
class Parser
{
    private readonly Token[] tokens;
    public Errors Diagnostics = new();
    private int position;
    private int count = 0;
    private readonly Dictionary<TokenType, Func<Expression>> tokenHandlers;

    public Parser(string text)
    {
        Lexer lexer = new(text);

        var tokens = lexer.Tokenize();

        this.tokens = tokens.ToArray();
        Diagnostics = lexer.diagnostics;

        tokenHandlers = new Dictionary<TokenType, Func<Expression>>
        {
            { TokenType.Number, ParseNumber },
            { TokenType.String, ParseString },
            { TokenType.Boolean, ParseBoolean },
            { TokenType.MathFunctions, ParseMathFunction },
            { TokenType.Keyword, ParseKeyword },
            { TokenType.Identificator, ParseIdentificator },
            { TokenType.LParen, ParseLParen },
            { TokenType.LBracket, ParseLBracket },
        };
    }
    
    private Token Peek(int offset)
    {
        var index = position + offset;
        if (index >= tokens.Length) return tokens[^1];
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

        if (Type == TokenType.RParen) Diagnostics.AddError($"! SYNTAX ERROR: Missing closing parenthesis after '{tokens[position - 1].Text}' (column {Current.Position + 1}).");
        else if (Type == TokenType.EOL) Diagnostics.AddError($"! SYNTAX ERROR: ';' expected ");
        else Diagnostics.AddError($"! SYNTAX ERROR: Invalid token '{Current.Text}', expected '{Type}' (column {Current.Position + 1}).");

        return new Token(TokenType.Error, Current.Position, string.Empty, string.Empty);
    }
    private Token Match(string Type)
    {
        if (Current.Text == Type) return NextToken();

        Diagnostics.AddError($"! SYNTAX ERROR: Invalid token '{Current.Text}', expected '{Type}' (column {Current.Position + 1}).");

        return new Token(TokenType.Error, Current.Position, string.Empty, string.Empty);
    }

    public SyntaxTree? Parse()
    {
        var expression = ParseExpression();

        var endOfFileToken = Match(TokenType.EOL);

        if (expression != null) return new SyntaxTree(Diagnostics, expression, endOfFileToken);

        return null;
    }
    public Expression? ParseExpression(int parentPrecedence = 0)
    {
        const int maxCount = 1000;
        if (count > maxCount)
        {
            Diagnostics.AddError($"! SYNTAX ERROR: Can't be parsed '{Current.Text}' (column {Current.Position + 1}).");
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
                Diagnostics.AddError($"! SYNTAX ERROR: Missing operand after '{operatorToken.Text}' operator (column {Current.Position + 1}).");
                return null;
            }
            left = new UnaryExpression(operatorToken, operand);
        }
        else left = ParsePrimaryExpression();

        int iterationCount = 0;
        while (iterationCount < maxCount)
        {
            var precedence = Current.Type.GetBinaryOperatorPrecedence();
            if (precedence == 0 || precedence <= parentPrecedence) break;

            var operatorToken = NextToken();
            var right = ParseExpression(precedence);
            if (left != null && right != null)
            {
                left = new BinaryExpression(left, operatorToken, right);
            }
            else{
                Diagnostics.AddError($"! SYNTAX ERROR: Invalid expression '{tokens[position - 1].Text}' (column {Current.Position + 1})");
                return null;
            }
            iterationCount++;
        }
        return left;
    }

    private Expression? ParsePrimaryExpression()
    {
        const int maxCount = 1000;

        if (count > maxCount)
        {
            Diagnostics.AddError($"! SYNTAX ERROR: Can't be parsed '{Current.Text}' (column {Current.Position + 1}).");
            return null;
        }

        if (tokenHandlers.TryGetValue(Current.Type, out var handler))
        {
            return handler();
        }
        return null;
    }
    private void ParseFunctionDeclaration()
    {
        Match("function");
        var name = Match(TokenType.Identificator);
        Match(TokenType.LParen);
        List<Token> arguments = new();
        if (Current.Type != TokenType.RParen) arguments.Add(Match(TokenType.Identificator));
        int overflow = 0;
        while (Current.Type != TokenType.RParen)
        {
            if (overflow > 1000)
            {
                Diagnostics.AddError($"! SYNTAX ERROR: Can't be parsed '{Current.Text}' (column {Current.Position + 1}).");
                return;
            }
            Match(TokenType.Comma);
            var argument = Match(TokenType.Identificator);
            if (argument.Text == string.Empty)
            {
                Diagnostics.AddError($"! SYNTAX ERROR: Missing argument in '{name.Text}' function (column {Current.Position + 1}).");
                return;
            }
            arguments.Add(argument);
            overflow++;
        }
        Match(TokenType.RParen);
        Match(TokenType.Asignation);
        Match(TokenType.Greater);
        var body = ParseExpression();
        if (body == null)
        {
            Diagnostics.AddError($"! SYNTAX ERROR: Missing expression after '=>' (column {Current.Position + 1}).");
            return;
        }

        foreach (var item in Evaluator.FunctionsScope)
        {
            if (item.Item1 == name.Text)
            {
                Diagnostics.AddError($"! PARSER ERROR: \"{name.Text}\" is already defined.");
                return;
            }
        }
        if (Diagnostics.AnyError()) return;

        Evaluator.FunctionsScope.Add(new Tuple<string, List<Token>, Expression>(name.Text, arguments, body));

        body.EvaluateExpression();
        if (!Evaluator.FunctionBody.Item1)
        {
            Diagnostics.AddError(Evaluator.FunctionBody.Item2, true);
            Evaluator.FunctionBody = (true, "");
            return;
        }

        if (InferenceTypes.GetInferenceType(body) == InferenceType.None)
        {
            Diagnostics.AddError($"! SEMANTIC ERROR: Can't infer the type of the arguments in '{name.Text}'.");
            return;
        }
    }
    private Expression ParseLBracket()
    {
        Match(TokenType.LBracket);
        List<Expression> elements = new();
        if (Current.Type != TokenType.RBracket)
        {
            var expression = ParseExpression();
            if (expression != null) elements.Add(expression);
            else Diagnostics.AddError("! SYNTAX ERROR: Missing expression in vector (column {Current.Position+1}).");
        }
        while (Current.Type != TokenType.RBracket)
        {
            Match(TokenType.Comma);
            var expression = ParseExpression();
            if (expression != null) elements.Add(expression);
            else Diagnostics.AddError("! SYNTAX ERROR: Missing expression in vector (column {Current.Position+1}).");
        }
        Match(TokenType.RBracket);

        return new VectorExpression(elements);
    }
    private Expression ParseNumber()
    {
        var numberToken = Match(TokenType.Number);
        return new NumberExpression(numberToken);
    }
    private Expression ParseString()
    {
        var stringToken = Match(TokenType.String);
        return new StringExpression(stringToken);
    }
    private Expression ParseBoolean()
    {
        var BoolToken = Match(TokenType.Boolean);
        return new BooleanExpression(BoolToken);
    }
    private Expression ParseLParen()
    {
        var left = NextToken();
        var expression = ParseExpression();
        var right = Match(TokenType.RParen);

        if (expression == null)
        {
            Diagnostics.AddError($"! SYNTAX ERROR: Missing expression in parenthesis (column {Current.Position + 1}).");
            return null;
        }
        return new ParenExpression(left, expression, right);
    }
    private Expression ParseMathFunction()
    {
        if (Current.Text == "log")
        {
            NextToken();
            Match(TokenType.LParen);
            var bas = ParseExpression();
            Match(TokenType.Comma);
            var number = ParseExpression();
            Match(TokenType.RParen);

            if (bas == null || number == null)
            {
                Diagnostics.AddError($"! SYNTAX ERROR: Missing expression in 'log' (column {Current.Position + 1}).");
                return null;
            }
            return new LogExpression(bas, number);
        }
        if (Current.Text == "range")
        {
            NextToken();
            Match(TokenType.LParen);
            var start = ParseExpression();
            if (start == null)
            {
                Diagnostics.AddError($"! SYNTAX ERROR: Missing expression after 'range' (column {Current.Position + 1}).");
                return null;
            }
            if (Current.Type == TokenType.Comma)
            {
                NextToken();
                var end = ParseExpression();
                if (end == null)
                {
                    Diagnostics.AddError($"! SYNTAX ERROR: Missing expression after ',' (column {Current.Position + 1}).");
                    return null;
                }
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

        if (Current.Text == "rand")
        {
            NextToken();

            Match(TokenType.LParen);
            Match(TokenType.RParen);

            return new NumberExpression(new Token(TokenType.Number, 0, "", new Random().NextDouble()));
        }

        var trigToken = NextToken();
        Match(TokenType.LParen);
        var expression = ParseExpression();
        if (expression == null)
        {
            Diagnostics.AddError($"! SYNTAX ERROR: Missing expression in '{trigToken.Text}' (column {Current.Position + 1}).");
            return null;
        }
        Match(TokenType.RParen);

        return new MathExpression(trigToken, expression);
    }
    private Expression ParseKeyword()
    {
        if (Current.Text == "print")
        {
            NextToken();
            Match(TokenType.LParen);
            var expression = ParseExpression();
            Match(TokenType.RParen);

            if (expression == null)
            {
                return new PrintExpression(new StringExpression(new Token(TokenType.String, 0, string.Empty, string.Empty)));
            }
            return new PrintExpression(expression);
        }
        if (Current.Text == "let")
        {
            List<Token> variablesNames = new();
            List<Expression> variablesExpressions = new();
            NextToken();
            variablesNames.Add(Match(TokenType.Identificator));
            Match(TokenType.Asignation);
            var expression = ParseExpression();
            if (expression == null)
            {
                Diagnostics.AddError($"! SYNTAX ERROR: Missing expression in 'let-in' after variable '{variablesNames[0].Text} (column {Current.Position + 1})'");
                return null;
            }
            variablesExpressions.Add(expression);
            Evaluator.VariableScope.Add(new Tuple<string, Expression, int>(variablesNames[0].Text, variablesExpressions[0], ++Evaluator.ScopePointer));
            if (Current.Type == TokenType.Comma)
            {
                do
                {
                    NextToken();
                    variablesNames.Add(Match(TokenType.Identificator));
                    Match(TokenType.Asignation); ;
                    var expression1 = ParseExpression();
                    if (expression1 == null)
                    {
                        Diagnostics.AddError($"! SYNTAX ERROR: Missing expression in 'let-in' after variable '{variablesNames[variablesNames.Count - 1].Text} (column {Current.Position + 1})'");
                        return null;
                    }
                    variablesExpressions.Add(expression1);
                    Evaluator.VariableScope.Add(new Tuple<string, Expression, int>(variablesNames[^1].Text, variablesExpressions[^1], Evaluator.ScopePointer));
                }
                while (Current.Type == TokenType.Comma);
                var inToken = Match("in");
                if (inToken.Type == TokenType.Error)
                {
                    Diagnostics.RemoveError();
                    Diagnostics.AddError($"! SYNTAX ERROR: Missing 'in' after 'let' expression (column {Current.Position + 1}).");
                }
                var inexpression = ParseExpression();

                if (inexpression == null)
                {
                    Diagnostics.AddError($"! SYNTAX ERROR: Missing expression after 'in' (column {Current.Position + 1}).");
                    return null;
                }
                return new LetInExpression(variablesNames, variablesExpressions, inexpression);
            }
            else
            {
                Match("in");
                var inExpression = ParseExpression();
                if (inExpression == null)
                {
                    Diagnostics.AddError($"! SYNTAX ERROR: Missing expression after 'in' (column {Current.Position + 1}).");
                    return null;
                }
                else return new LetInExpression(variablesNames, variablesExpressions, inExpression);
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
                if (body == null)
                {
                    Diagnostics.AddError($"! SYNTAX ERROR: Missing expression after 'for' (column {Current.Position + 1}).");
                    return null;
                }
                else return new ForExpression(identifier, null, name, body);
            }
            else
            {
                Match("range");
                Match(TokenType.LParen);
                var start = ParseExpression();
                if (start == null)
                {
                    Diagnostics.AddError($"! SYNTAX ERROR: Missing expression after 'range' (column {Current.Position + 1}).");
                    return null;
                }
                if (Current.Type == TokenType.Comma)
                {
                    Match(TokenType.Comma);
                    var end = ParseExpression();
                    if (end == null)
                    {
                        Diagnostics.AddError($"! SYNTAX ERROR: Missing expression after ',' (column {Current.Position + 1}).");
                        return null;
                    }
                    Match(TokenType.RParen);
                    Match(TokenType.RParen);
                    var body = ParseExpression();
                    if (body == null)
                    {
                        Diagnostics.AddError($"! SYNTAX ERROR: Missing expression after 'for' (column {Current.Position + 1}).");
                        return null;
                    }
                    return new ForExpression(identifier, new RangeFunction(start, end), null, body);
                }
                Match(TokenType.RParen);
                Match(TokenType.RParen);
                var body1 = ParseExpression();
                if (body1 == null)
                {
                    Diagnostics.AddError($"! SYNTAX ERROR: Missing expression after 'for' (column {Current.Position + 1}).");
                    return null;
                }
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
            if (condition == null)
            {
                Diagnostics.AddError($"! SYNTAX ERROR: Missing 'if' condition (column {Current.Position + 1}).");
                return null;
            }
            Match(TokenType.RParen);
            var ifexpression = ParseExpression();
            if (ifexpression == null)
            {
                Diagnostics.AddError($"! SYNTAX ERROR: Missing expression after 'if' (column {Current.Position + 1}).");
                return null;
            }
            while (Current.Text == "elif")
            {
                NextToken();
                Match(TokenType.LParen);
                var expression = ParseExpression();
                if (expression == null)
                {
                    Diagnostics.AddError($"! SYNTAX ERROR: Missing 'elif' condition (column {Current.Position + 1}).");
                    return null;
                }
                elifcondition.Add(expression);
                Match(TokenType.RParen);
                var elifExpression = ParseExpression();
                if (elifExpression == null)
                {
                    Diagnostics.AddError($"! SYNTAX ERROR: Missing expression after 'elif' (column {Current.Position + 1}).");
                    return null;
                }
                elifs.Add(elifExpression);
            }
            Match("else");
            var elseexpression = ParseExpression();

            if (elseexpression == null)
            {
                Diagnostics.AddError($"! SYNTAX ERROR: Missing expression after 'else' (column {Current.Position + 1}).");
                return null;
            }
            return new IfExpression(condition, ifexpression, elifcondition, elifs, elseexpression);
        }
        if (Current.Text == "function")
        {
            ParseFunctionDeclaration();
            return null;
        }
        if (Current.Text == "while")
        {
            NextToken();
            Match(TokenType.LParen);
            var condition = ParseExpression();
            if (condition == null)
            {
                Diagnostics.AddError($"! SYNTAX ERROR: Missing 'while' condition (column {Current.Position + 1}).");
                return null;
            }
            Match(TokenType.RParen);
            var body = ParseExpression();
            if (body == null)
            {
                Diagnostics.AddError($"! SYNTAX ERROR: Missing expression after 'while' (column {Current.Position + 1}).");
                return null;
            }

            return new WhileExpression(condition, body);
        }
        return null;
    }
    private Expression ParseIdentificator()
    {
        var name = Match(TokenType.Identificator);

        if (Current.Type == TokenType.LParen)
        {
            Match(TokenType.LParen);
            List<Expression> arguments = new();
            if (Current.Type != TokenType.RParen)
            {
                var expression = ParseExpression();
                if (expression != null)
                {
                    arguments.Add(expression);
                }
            }
            while (Current.Type != TokenType.RParen && Current.Type != TokenType.EOL)
            {
                Match(TokenType.Comma);
                if (Diagnostics.AnyError())
                {
                    Diagnostics.RemoveError();
                    Diagnostics.AddError($"! SYNTAX ERROR: Missing closing parenthesis in '{name.Text}' call (column {Current.Position + 1}).");
                    return null;
                }
                var expression = ParseExpression();
                if (expression != null)
                {
                    arguments.Add(expression);
                }
            }
            Match(TokenType.RParen);
            return new FunctionCallExpression(name, arguments);
        }

        if (Current.Type == TokenType.LBracket)
        {
            Match(TokenType.LBracket);
            var index = ParseExpression();
            Match(TokenType.RBracket);

            return new NumberExpression(new Token(TokenType.Number, 0, "", VectorExpression.GetElement(name, Convert.ToInt32(Evaluator.Evaluate(index)))));
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

}