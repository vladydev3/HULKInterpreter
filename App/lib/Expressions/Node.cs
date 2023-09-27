namespace hulk;

public abstract class Node
{
    public abstract TokenType Type { get; }
    public abstract IEnumerable<Node> GetChildren();
}

public abstract class Expression : Node
{
    public abstract object EvaluateExpression();
    public Errors Diagnostics = new();
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