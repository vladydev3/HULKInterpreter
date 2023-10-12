namespace hulk;

public class SyntaxTree
{
    public SyntaxTree(Errors diagnostics, Expression root, Token endOfFileToken)
    {
        Root = root;
        EndOfFileToken = endOfFileToken;
        Diagnostics = diagnostics;
    }
    public Errors Diagnostics;
    public Expression Root { get; }
    public Token EndOfFileToken { get; }

    public static SyntaxTree Parse(string text)
    {
        var parser = new Parser(text);
        return parser.Parse();
    }
}