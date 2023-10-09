namespace hulk;

public sealed class StringExpression : Expression
{
    public override TokenType Type => TokenType.String;
    public Token StringToken;

    public StringExpression(Token stringToken)
    {
        StringToken = stringToken;
    }

    public override string EvaluateExpression()
    {
        StringToken.Text = StringToken.Text.Substring(1, StringToken.Text.Length - 2);
        StringToken.Text = StringToken.Text.Replace("\\t", "\t");
        StringToken.Text = StringToken.Text.Replace("\\n", "\n");
        StringToken.Text = StringToken.Text.Replace("\\", "");

        return StringToken.Text;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return StringToken;
    }
}

public sealed class BooleanExpression : Expression
{
    public override TokenType Type => TokenType.Boolean;
    public Token Bool { get; }

    public BooleanExpression(Token boolean)
    {
        Bool = boolean;
    }

    public override object EvaluateExpression()
    {
        return Bool.Value;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return Bool;
    }
}

public sealed class NumberExpression : Expression
{
    public override TokenType Type => TokenType.Number;
    public Token NumberToken { get; }

    public NumberExpression(Token numberToken)
    {
        NumberToken = numberToken;
    }

    public override object EvaluateExpression()
    {
        return NumberToken.Value;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return NumberToken;
    }
}

public sealed class VariableExpression : Expression
{
    public override TokenType Type => TokenType.Identificator;
    public Token VariableName;

    public VariableExpression(Token variableName)
    {
        VariableName = variableName;
    }

    public override object EvaluateExpression()
    {
        for (int i = Evaluator.VariableScope.Count - 1; i >= 0; i--)
        {
            if (Evaluator.VariableScope[i].Item1 == VariableName.Text)
            {
                return Evaluator.VariableScope[i].Item2.EvaluateExpression();
            }
        }
        Evaluator.Diagnostics.AddError($"Semantic Error: Variable \"{VariableName.Text}\" is not defined");
        return null;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return VariableName;
    }
}

public class VectorExpression : Expression
{
    public override TokenType Type => TokenType.VectorExpression;
    public List<Expression> Elements { get; }
    public int Current { get; set; }

    public VectorExpression(List<Expression> elements)
    {
        Elements = elements;
    }

    public object GetCurrent(Token name)
    {
        return GetElement(name, Current);
    }

    public bool GetNext(Token name)
    {
        return ++Current < Elements.Count;
    }

    public static VectorExpression GetVector(Token name)
    {
        try
        {
            for (int i = Evaluator.VariableScope.Count - 1; i >= 0; i--)
            {
                if (Evaluator.VariableScope[i].Item1 == name.Text)
                {
                    return (VectorExpression)Evaluator.VariableScope[i].Item2;
                }
            }
            Evaluator.Diagnostics.AddError($"Semantic Error: Variable \"{name.Text}\" is not defined");
        }
        catch (Exception)
        {
            Evaluator.Diagnostics.AddError($"Semantic Error: Variable \"{name.Text}\" is not a vector");
        }
        return null;
    }

    public static object GetElement(Token name, int index)
    {
        try
        {
            for (int i = Evaluator.VariableScope.Count - 1; i >= 0; i--)
            {
                if (Evaluator.VariableScope[i].Item1 == name.Text)
                {
                    return ((List<object>)Evaluator.VariableScope[i].Item2.EvaluateExpression())[index];
                }
            }
            Evaluator.Diagnostics.AddError($"Semantic Error: Variable \"{name.Text}\" is not defined");
        }
        catch (Exception)
        {
            Evaluator.Diagnostics.AddError($"Semantic Error: Variable \"{name.Text}\" is not a vector");
        }
        return null;
    }

    public override List<object> EvaluateExpression()
    {
        var vector = new List<object>();
        foreach (var element in Elements)
        {
            vector.Add(Evaluator.Evaluate(element));
        }
        return vector;
    }

    public override IEnumerable<Node> GetChildren()
    {
        foreach (var element in Elements)
        {
            yield return element;
        }
    }
}