namespace test;
using hulk;

[TestFixture]
public class BasicExpressions
{
    [Test]
    public void String()
    {
        string code = "print(\"Oracion blabla\" @ \"okok\");";
        var tree = SyntaxTree.Parse(code);

        var result = Evaluator.Evaluate(tree.Root);

        Assert.That(result, Is.EqualTo("Oracion blablaokok"));
    }

    [Test]
    public void String2()
    {
        var code = "print(\"Hola\nSalto\t de linea\");";
        var tree = SyntaxTree.Parse(code);

        var result = Evaluator.Evaluate(tree.Root);

        Assert.That(result, Is.EqualTo("Hola\nSalto\t de linea"));
    }

    [Test]
    public void ArithmeticExpressions()
    {
        string code = "print((((1 + 2) ^ 3) * 4) / 5);";
        var tree = SyntaxTree.Parse(code);

        var result = Evaluator.Evaluate(tree.Root);

        Assert.That(result, Is.EqualTo(21.6));
    }
    
    [Test]
    public void TrigFunctions()
    {
        string code = "print(sin(2 * PI) ^ 2 + cos(3 * PI / log(4, 64)));";
        var tree = SyntaxTree.Parse(code);

        var result = Evaluator.Evaluate(tree.Root);

        Assert.That(result, Is.EqualTo(-1));
    }

    [Test]
    public void LetIn()
    {
        string code = "let a =( let b = 5 in b ) in a+b;";
        var tree = SyntaxTree.Parse(code);

        var result = Evaluator.Evaluate(tree.Root);
        var error = Evaluator.Diagnostics.AnyError();

        Assert.That(error, Is.EqualTo(true));
    }
}

[TestFixture]
public class Functions
{
    [Test]
    public void Tan()
    {
        string code = "function tan(x) => sin(x) / cos(x);";
        var tree = SyntaxTree.Parse(code);

        string code2 = "print(tan(1));";
        var tree2 = SyntaxTree.Parse(code2);
        var result = Evaluator.Evaluate(tree2.Root);

        Assert.That(result, Is.EqualTo(1.5574077246549021));
    }
    [Test]
    public void Recursion()
    {
        string code = "function fib(n) => if (n > 1) fib(n-1) + fib(n-2) else 1;";
        var tree = SyntaxTree.Parse(code);

        string code2 = "let x = 3 in fib(x+1);";
        var tree2 = SyntaxTree.Parse(code2);
        var result = Evaluator.Evaluate(tree2.Root);

        Assert.That(result, Is.EqualTo(5));

        string code3 = "fib(6);";
        var tree3 = SyntaxTree.Parse(code3);
        var result2 = Evaluator.Evaluate(tree3.Root);

        Assert.That(result2, Is.EqualTo(13));
    }

    [Test]
    public void Recursion2()
    {
        string code = "function mcd(a,b) => if (a % b ==0) b else mcd(b, a%b);";
        var tree = SyntaxTree.Parse(code);

        string code2 = "mcd(36, 24);";
        var tree2 = SyntaxTree.Parse(code2);
        var result = Evaluator.Evaluate(tree2.Root);

        Assert.That(result, Is.EqualTo(12));    

        string code3 = "mcd(8, 4);";
        var tree3 = SyntaxTree.Parse(code3);
        var result2 = Evaluator.Evaluate(tree3.Root);

        Assert.That(result2, Is.EqualTo(4));
    }
}

[TestFixture]
public class Operators
{
    [Test]
    public void Concat()
    {
        string code = "let number = 42, text = \"The meaning of life is\" in print(text @ number);";
        var tree = SyntaxTree.Parse(code);
        
        var result = Evaluator.Evaluate(tree.Root);

        Assert.That(result, Is.EqualTo("The meaning of life is42"));
    }
    [Test]
    public void BooleanTrue()
    {
        string code = "print(3>3|4<4+2);";
        var tree = SyntaxTree.Parse(code);

        var result = Evaluator.Evaluate(tree.Root);
        
        Assert.That(result, Is.EqualTo(true));
    }
    [Test]
    public void BooleanFalse()
    {
        string code = "print(3<=1 & 0==2);";
        var tree = SyntaxTree.Parse(code);

        var result = Evaluator.Evaluate(tree.Root);
        
        Assert.That(result, Is.EqualTo(false));
    }
    [Test]
    public void UnaryOperators()
    {
        string code = "print(!(2==2));";
        var tree = SyntaxTree.Parse(code);

        var result = Evaluator.Evaluate(tree.Root);
        
        Assert.That(result, Is.EqualTo(false));

        string code1 = "print(!(2==2));";
        var tree1 = SyntaxTree.Parse(code1);

        var result1 = Evaluator.Evaluate(tree1.Root);
        
        Assert.That(result1, Is.EqualTo(false));
    }
}

[TestFixture]
public class Conditionals
{
    [Test]
    public void Case1()
    {
        string code = "print(let a = 42 in if (a % 2 == 0) print(\"Even\") else print(\"odd\"));";
        var tree = SyntaxTree.Parse(code);

        var result = Evaluator.Evaluate(tree.Root);

        Assert.That(result, Is.EqualTo("Even"));
    }

    [Test]
    public void Case2()
    {
        string code = "print(let a = 42 in print(if (a % 2 != 0) \"even\" else \"odd\"));";
        var tree = SyntaxTree.Parse(code);

        var result = Evaluator.Evaluate(tree.Root);

        Assert.That(result, Is.EqualTo("odd"));
    }
}

