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

    public static void Debug(string msg)
    {
        WriteLine(msg, ConsoleColor.White);
    }

    public static void Info(string msg)
    {
        WriteLine(msg, ConsoleColor.Green);
    }

    public static void Warn(string msg)
    {
        WriteLine(msg, ConsoleColor.Yellow);
    }

    public static void Error(string msg)
    {
        WriteLine(msg, ConsoleColor.Red);
    }
}