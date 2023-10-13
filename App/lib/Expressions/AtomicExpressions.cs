namespace hulk;

public class StringExpression : Expression
{
    public override TokenType Type => TokenType.String;
    public Token StringToken;

    public StringExpression(Token stringToken)
    {
        StringToken = stringToken;
    }

    public override string? EvaluateExpression()
    {
        StringToken.Value = StringToken.Value.ToString().Replace("\\t", "\t");
        StringToken.Value = StringToken.Value.ToString().Replace("\\n", "\n");
        StringToken.Value = StringToken.Value.ToString().Replace("\\", "");
        return StringToken.Value.ToString();
    }
}

public class BooleanExpression : Expression
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
}

public class NumberExpression : Expression
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
}

public class VariableExpression : Expression
{
    public override TokenType Type => TokenType.Identificator;
    public Token VariableName;
    public int Scope;

    public VariableExpression(Token variableName, int scope)
    {
        VariableName = variableName;
        Scope = scope;
    }

    public override object EvaluateExpression()
    {
        for (int i = Evaluator.VariableScope.Count - 1; i >= 0; i--)
        {
            if (Evaluator.VariableScope[i].Item1 == VariableName.Text && Evaluator.VariableScope[i].Item3 >= Scope)
            {
                return Evaluator.VariableScope[i].Item2.EvaluateExpression();
            }
        }
        if (Evaluator.FunctionsScope.Count > 0)
        {
            foreach (var argument in Evaluator.FunctionsScope[Evaluator.FunctionsScope.Count - 1].Item2)
            {
                if (argument.Text == VariableName.Text)
                {
                    return "";
                }
            }
            Evaluator.FunctionBody = (false, $"! SEMANTIC ERROR: Variable \"{VariableName.Text}\" is not defined");
        }

        Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Variable \"{VariableName.Text}\" is not defined");
        return "";
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
        return GetElement(name, Current++);
    }

    public bool GetNext()
    {
        return Current < Elements.Count;

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
            Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Variable \"{name.Text}\" is not defined");
        }
        catch (Exception)
        {
            Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Variable \"{name.Text}\" is not a vector");
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
            Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Variable \"{name.Text}\" is not defined");
        }
        catch (Exception)
        {
            Evaluator.Diagnostics.AddError($"! SEMANTIC ERROR: Variable \"{name.Text}\" is not a vector");
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
}