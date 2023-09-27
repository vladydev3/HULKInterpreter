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
                var e = new Evaluator(syntaxTree.Root);
                var result = e.Evaluate();
                ConsoleColor color = Console.ForegroundColor;
                if (e.Diagnostics.Count != 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                }
                Console.WriteLine(result);
                Console.ForegroundColor = color;
            }
        }
    }
}