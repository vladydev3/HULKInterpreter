namespace test;
using hulk;

[TestFixture]
public class BasicExpressions
{
    [Test]
    public void String()
    {
        // Arrange
        string code = "print(\"Hello\");";
        var tree = SyntaxTree.Parse(code);

        // Act
        var result = Evaluator.Evaluate(tree.Root);

        // Assert
        Assert.AreEqual("Hello", result);
    }

    [Test]
    public void ArithmeticExpressions()
    {
        string code = "print((((1 + 2) ^ 3) * 4) / 5);";
        var tree = SyntaxTree.Parse(code);

        var result = Evaluator.Evaluate(tree.Root);

        Assert.AreEqual(21.6, result);
    }
    
    [Test]
    public void TrigFunctions()
    {
        string code = "print(sin(2 * PI) ^ 2 + cos(3 * PI / log(4, 64)));";
        var tree = SyntaxTree.Parse(code);

        var result = Evaluator.Evaluate(tree.Root);

        Assert.AreEqual(-1, result);
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

        Assert.AreEqual(false, tree.Diagnostics.AnyError());

        string code2 = "print(tan(1));";
        var tree2 = SyntaxTree.Parse(code2);
        var result = Evaluator.Evaluate(tree2.Root);

        Assert.AreEqual(1.557407724654902, result);
    }
    [Test]
    public void Recursion()
    {
        string code = "function fib(n) => if (n > 1) fib(n-1) + fib(n-2) else 1;";
        var tree = SyntaxTree.Parse(code);

        Assert.AreEqual(false, tree.Diagnostics.AnyError());

        string code2 = "let x = 3 in fib(x+1);";
        var tree2 = SyntaxTree.Parse(code2);
        var result = Evaluator.Evaluate(tree2.Root);

        Assert.AreEqual(5, result);

        string code3 = "fib(6);";
        var tree3 = SyntaxTree.Parse(code3);
        var result2 = Evaluator.Evaluate(tree3.Root);

        Assert.AreEqual(13, result2);
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

        Assert.AreEqual("The meaning of life is42", result);
    }
    [Test]
    public void BooleanTrue()
    {
        string code = "print(3>3|4<4+2);";
        var tree = SyntaxTree.Parse(code);

        var result = Evaluator.Evaluate(tree.Root);
        
        Assert.AreEqual(true, result);
    }
    [Test]
    public void BooleanFalse()
    {
        string code = "print(3<=1 & 0==2);";
        var tree = SyntaxTree.Parse(code);

        var result = Evaluator.Evaluate(tree.Root);
        
        Assert.AreEqual(false, result);
    }
    [Test]
    public void UnaryOperators()
    {
        string code = "print(!(2==2));";
        var tree = SyntaxTree.Parse(code);

        var result = Evaluator.Evaluate(tree.Root);
        
        Assert.AreEqual(false, result);

        string code1 = "print(!(2==2));";
        var tree1 = SyntaxTree.Parse(code1);

        var result1 = Evaluator.Evaluate(tree1.Root);
        
        Assert.AreEqual(false, result1);
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

        Assert.AreEqual("Even", result);
    }

    [Test]
    public void Case2()
    {
        string code = "print(let a = 42 in print(if (a % 2 != 0) \"even\" else \"odd\"));";
        var tree = SyntaxTree.Parse(code);

        var result = Evaluator.Evaluate(tree.Root);

        Assert.AreEqual("odd", result);
    }
}

