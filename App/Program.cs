using hulk;

class Program
{
    static void Main()
    {
        var clr = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"HULK Compiler");
        Console.WriteLine($"-----------------------------");
        Console.ForegroundColor = clr;
        while (true)
        {
            Evaluator.VariableScope = new List<Tuple<string, Expression>>();
            Console.Write(">>> ");
            string code = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(code)) return;

            else if (code == "#clear")
            {
                Console.Clear();
                continue;
            }

            else if (code == "#exit") return;

            var syntaxTree = SyntaxTree.Parse(code);

            if (syntaxTree.Diagnostics.AnyError()) syntaxTree.Diagnostics.ShowError();
            else
            {
                if (syntaxTree.Root == null) continue;
                var result = Evaluator.Evaluate(syntaxTree.Root);
                if (syntaxTree.Root.Diagnostics.AnyError()) syntaxTree.Root.Diagnostics.ShowError();
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
    }
}