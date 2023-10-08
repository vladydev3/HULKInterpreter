using hulk;


var clr = Console.ForegroundColor;
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"HULK Interpreter");
Console.WriteLine($"-----------------------------");
Console.ForegroundColor = clr;
while (true)
{
    Evaluator.VariableScope = new List<Tuple<string, Expression>>();
    Evaluator.PrintResult = false;
    Console.Write("> ");
    string code = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(code)) return;

    else if (code == "#clear")
    {
        Console.Clear();
        continue;
    }

    var syntaxTree = SyntaxTree.Parse(code);

    if (syntaxTree.Diagnostics.AnyError()) syntaxTree.Diagnostics.ShowError();
    else
    {
        if (syntaxTree.Root == null) continue;   
        var result = Evaluator.Evaluate(syntaxTree.Root);

        if (Evaluator.Diagnostics.AnyError()) Evaluator.Diagnostics.ShowError();
        else
        {
            if (result != null && Evaluator.PrintResult)
            {
                Console.WriteLine(result);
            }
        }
    }
}