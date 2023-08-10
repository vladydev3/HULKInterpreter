using System;
using Backend;

class Program
{
    static void Main()
    {
        Console.WriteLine("**Intérprete de HULK**");
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("Escriba 'exit' para salir del intérprete...\n");
        while (true)
        {
            Console.Write(">>> ");
            string a = Console.ReadLine();
            if (a == "exit")
            {
                Console.WriteLine("Saliendo...");
                break;
            }

            try
            {
                var lexer = new Backend.Lexer();
                var tokens = lexer.Tokenize(a);

                RecursiveParser parser = new RecursiveParser();
                Console.WriteLine(parser.Parse(tokens));
                


                foreach (var token in tokens)
                {
                    Console.WriteLine($"{token.Type}: {token.Value}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
    }
}