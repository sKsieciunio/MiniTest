namespace MiniTestRunner;

public static class Utils
{
    public static void ConsoleWriteColorLine(string message, ConsoleColor color = ConsoleColor.Gray, int indent = 0)
    {
        var indentStr = new string(' ', indent);
        Console.ForegroundColor = color;
        Console.WriteLine($"{indentStr}{message}");
        Console.ResetColor();
    }
    
    public static void ConsoleWriteColor(string message, ConsoleColor color = ConsoleColor.Gray, int indent = 0)
    {
        var indentStr = new string(' ', indent);
        Console.ForegroundColor = color;
        Console.Write($"{indentStr}{message}");
        Console.ResetColor();
    }
}