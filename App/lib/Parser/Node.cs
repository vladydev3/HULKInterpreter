namespace hulk;

public abstract class Node
{
    public abstract TokenType Type {get;}
    public abstract IEnumerable<Node> GetChildren();     
}

public abstract class Expression : Node
{
    //
}

public sealed class NumberExpression : Expression
{
    public override TokenType Type => TokenType.Number;
    public Token NumberToken { get; }

    public NumberExpression(Token numberToken)
    {
        NumberToken = numberToken;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return NumberToken;
    }
}

public sealed class BinaryExpression : Expression
{
    public override TokenType Type => TokenType.BinaryExpression;
    public Expression Left { get; }
    public Token Operator { get; }
    public Expression Right { get; }

    public BinaryExpression(Expression left, Token operatorToken, Expression right)
    {
        Left = left;
        Operator = operatorToken;
        Right = right;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return Left;
        yield return Operator;
        yield return Right;
    }
}

public sealed class UnaryExpression : Expression
{

    public override TokenType Type => TokenType.UnaryExpression;
    public Token OperatorToken { get; }
    public Expression Operand { get; }
    public UnaryExpression(Token operatorToken, Expression operand)
    {
        OperatorToken = operatorToken;
        Operand = operand;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return OperatorToken;
        yield return Operand;
    }
}

public class PrintExpression : Expression
{
    public override TokenType Type => TokenType.PrintExpression;
    public Token PrintToken;
    public ParenExpression ExpressionInside;  

    public PrintExpression(Token printToken, Token openParen, Expression expression, Token closeParen)
    {
        PrintToken = printToken;
        ExpressionInside = new ParenExpression(openParen, expression, closeParen);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return ExpressionInside;
    }
}

public sealed class ParenExpression : Expression
{
    public override TokenType Type => TokenType.Parenthesis;
    public Token OpenParen {get;}
    public Expression Expression {get;}
    public Token CloseParen {get;}
    
    public ParenExpression(Token openParen, Expression expression, Token closeParen)
    {
        OpenParen = openParen;
        Expression = expression;
        CloseParen = closeParen;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return OpenParen;
        yield return Expression;
        yield return CloseParen;
    }
}

sealed class SyntaxTree
{
    public SyntaxTree(IEnumerable<string> diagnostics, Expression root, Token endOfFileToken)
    {
        Root = root;
        EndOfFileToken = endOfFileToken;
        Diagnostics = diagnostics.ToList();
    }
    public List<string> Diagnostics;
    public Expression Root { get; }
    public Token EndOfFileToken { get; }

    public static SyntaxTree Parse(string text)
    {
        var parser = new Parser(text);
        return parser.Parse();
    }
}


// /*
// <program> ::= <expression> | <function>
// <expression> ::= <print_statement> | <arithmetic_expression> | <function_call> | <let_in_expression> | <if_else_expression>
// <print_statement> ::= "print(" <expression> ");"
// <arithmetic_expression> ::= <number> | <arithmetic_expression> "+" <arithmetic_expression> | <arithmetic_expression> "-" <arithmetic_expression> | <arithmetic_expression> "*" <arithmetic_expression> | <arithmetic_expression> "/" <arithmetic_expression> | "(" <arithmetic_expression> ")"
// <function_call> ::= <function_name> "(" <expression> ")"
// <let_in_expression> ::= "let" <variable_declaration> "in" <expression>
// <variable_declaration> ::= <variable_name> "=" <expression> | <variable_name> "=" <expression> "," <variable_declaration>
// <if_else_expression> ::= "if" "(" <boolean_expression> ")" <expression> "else" <expression>
// <function> ::= "function" <function_name> "(" <parameter> ")" "=>" <expression>
// <parameter> ::= <variable_name> | <variable_name> "," <parameter>

// <number> ::= <integer> | <float>
// <integer> ::= <digit> | <digit> <integer>
// <float> ::= <integer> "." <integer>
// <digit> ::= "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"

// <function_name> ::= <letter> | <letter> <function_name_character>
// <function_name_character> ::= <letter> | <digit> | "_"

// <variable_name> ::= <letter> | <letter> <variable_name_character>
// <variable_name_character> ::= <letter> | <digit> | "_"

// <boolean_expression> ::= <arithmetic_expression> "<" <arithmetic_expression> | <arithmetic_expression> ">" <arithmetic_expression> | <arithmetic_expression> "==" <arithmetic_expression> | <arithmetic_expression> "!=" <arithmetic_expression>

// <letter> ::= "a" | "b" | "c" | "d" | "e" | "f" | "g" | "h" | "i" | "j" | "k" | "l" | "m" | "n" | "o" | "p" | "q" | "r" | "s" | "t" | "u" | "v" | "w" | "x" | "y" | "z" | "A" | "B" | "C" | "D" | "E" | "F" | "G" | "H" | "I" | "J" | "K" | "L" | "M" | "N" | "O" | "P" | "Q" | "R" | "S" | "T" | "U" | "V" | "W" | "X" | "Y" | "Z"
// */