namespace hulk;

public class Errors
{
    public List<string> diagnostics = new();

    public bool AnyError()
    {
        return !(diagnostics.Count==0);
    }

    public void ShowError()
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine(diagnostics[0]);
        diagnostics = new();
        Console.ForegroundColor = color;
    }

    public void AddError(string error)
    {
        diagnostics.Add(error);
    }

    public void RemoveError()
    {
        diagnostics.RemoveAt(diagnostics.Count-1);
    }
}