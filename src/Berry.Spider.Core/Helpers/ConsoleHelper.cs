using Console = System.Console;

namespace Berry.Spider.Core;

public static class ConsoleHelper
{
    public static void WriteLine(string msg, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    public static void Write(string msg, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.Write(msg);
        Console.ResetColor();
    }
}