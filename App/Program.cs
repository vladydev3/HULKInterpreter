using hulk;


var clr = Console.ForegroundColor;
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"HULK Interpreter. Write your code and press enter to execute.");
Console.WriteLine($"Type #clear to clear the console.");
Console.WriteLine($"-----------------------------------------------------------------");
Console.ForegroundColor = clr;
while (true)
{
    Evaluator.VariableScope = new List<Tuple<string, Expression, int>>();
    Evaluator.Diagnostics = new Errors();
    Evaluator.ScopePointer = 0;
    Console.Write("> ");
    string? code = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(code)) return;

    else if (code == "#clear")
    {
        Console.Clear();
        continue;
    }

    var watch = System.Diagnostics.Stopwatch.StartNew();

    var syntaxTree = SyntaxTree.Parse(code);

    if (syntaxTree == null) continue;
    if (syntaxTree.Diagnostics.AnyError()) syntaxTree.Diagnostics.ShowError();
    else
    {
        var result = Evaluator.Evaluate(syntaxTree.Root);

        watch.Stop();
        if (Evaluator.Diagnostics.AnyError())
        {
            Evaluator.Diagnostics.ShowError();
            continue;
        }
        var elapsedMs = watch.ElapsedMilliseconds;
        var c = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"~ Execution time: {elapsedMs}ms");
        Console.ForegroundColor = c;

        if (result != null)
        {
            Console.WriteLine(result);
        }
    }
}