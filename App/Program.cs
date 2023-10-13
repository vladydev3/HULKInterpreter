using hulk;


var clr = Console.ForegroundColor;
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"HULK Interpreter");
Console.WriteLine($"-----------------------------");
Console.ForegroundColor = clr;
while (true)
{
    Evaluator.VariableScope = new List<Tuple<string, Expression, int>>();
    Evaluator.Diagnostics = new Errors();
    Evaluator.ScopePointer = 0;
    Console.Write("> ");
    string code = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(code)) return;

    else if (code == "#clear")
    {
        Console.Clear();
        continue;
    }

    var watch = System.Diagnostics.Stopwatch.StartNew();

    var syntaxTree = SyntaxTree.Parse(code);

    if (syntaxTree.Diagnostics.AnyError())
    {
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        var c = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"~ Execution time: {elapsedMs}ms");
        Console.ForegroundColor = c;
        syntaxTree.Diagnostics.ShowError();
    }
    else
    {
        if (syntaxTree.Root == null) continue;
        var result = Evaluator.Evaluate(syntaxTree.Root);

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        var c = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"~ Execution time: {elapsedMs}ms");
        Console.ForegroundColor = c;

        if (Evaluator.Diagnostics.AnyError()) Evaluator.Diagnostics.ShowError();
        else
        {
            if (result != null)
            {
                Console.WriteLine(result);
            }
        }
    }
}