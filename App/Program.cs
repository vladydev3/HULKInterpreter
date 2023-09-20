using hulk;

class Program{
    static void Main(){
        Console.WriteLine($"HULK Compiler");
        Console.WriteLine($"-----------------------------");
        while(true){
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
            
            if (!syntaxTree.Diagnostics.Any())
            {
                var e = new Evaluator(syntaxTree.Root);
                var result = e.Evaluate();
                Console.WriteLine(result);
            }
            else {
                var color = Console.ForegroundColor; 
                Console.ForegroundColor = ConsoleColor.DarkRed;

                foreach (var diagnostic in syntaxTree.Diagnostics)
                {
                    Console.WriteLine(diagnostic);
                }
                Console.ForegroundColor = color;
            }
        }
    }
}