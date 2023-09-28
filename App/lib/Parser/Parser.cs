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
    public static int GetBinaryOperatorPrecedence(this TokenType kind)
    {
        switch (kind)
        {
            case TokenType.Pow:
                return 4;
            case TokenType.Mult:
            case TokenType.Div:
            case TokenType.Mod:
                return 3;
            case TokenType.Plus:
            case TokenType.Minus:
                return 2;
            case TokenType.Concat:
            case TokenType.Bigger:
            case TokenType.BiggerOrEqual:
            case TokenType.Minor:
            case TokenType.MinorOrEqual:
            case TokenType.Comparation:
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

        Diagnostics.AddError($"Parser Error: Unexpected token <{Current.Type}>, expected <{Type}>");

        return new Token(Type, Current.Position, null, null);
    }

    public SyntaxTree Parse()
    {
        var expression = ParseExpression();
        var endOfFileToken = Match(TokenType.EOL);
        return new SyntaxTree(Diagnostics, expression, endOfFileToken);
    }
    private Expression ParseExpression(int parentPrecedence = 0)
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
                var let = Match(TokenType.Keyword);
                var variableName = Match(TokenType.Identificator);
                var equal = Match(TokenType.Asignation);
                var expression = ParseExpression();
                if (Current.Type == TokenType.Comma)
                {
                    var comma = Match(TokenType.Comma);
                    var let2 = Match(TokenType.Keyword);
                    var variableName2 = Match(TokenType.Identificator);
                    var equal2 = Match(TokenType.Asignation); ;
                    var expression2 = ParseExpression();
                    Evaluator.VariableScope.Add(new Tuple<string, Expression>(variableName.Text, expression));
                    Evaluator.VariableScope.Add(new Tuple<string, Expression>(variableName2.Text, expression2));
                    var intoken = Match(TokenType.Keyword);
                    var inexpression = ParseExpression();
                    return new LetInExpression(variableName, variableName2, expression, expression2, inexpression)      ;
                }
                else
                {
                    var inToken = Match(TokenType.Keyword);
                    var inExpression = ParseExpression();
                    Evaluator.VariableScope.Add(new Tuple<string, Expression>(variableName.Text, expression));
                    return new LetInExpression(variableName, null, expression, null, inExpression);
                }

            }
        }

        if (Current.Type == TokenType.Identificator)
        {
            var name = Match(TokenType.Identificator);

            return new VariableExpression(name);
        }

        return null;
    }
}



