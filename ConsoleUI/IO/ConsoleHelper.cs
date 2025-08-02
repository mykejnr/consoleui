namespace ConsoleUI.IO;

public class ConsoleHelper
{
    public static void SingleUnderline(int width = 50)
    {
        Console.WriteLine("".PadRight(width, '-'));
    }

    public static void DoubleUnderline(int width = 50)
    {
        Console.WriteLine("".PadRight(width, '='));
    }

    public static void PrintHeading(string title, params string[] subTitles)
    {
        Console.WriteLine(title);
        foreach (var t in subTitles) Console.WriteLine(t);
        SingleUnderline();
    }

    /// <summary>
    /// Displays a message to the console and waits for the user to
    /// pnter any key before before, optionally clearing the screen,
    /// and moving on
    /// </summary>
    /// <param name="msg">Message to display</param>
    /// <param name="clearAfter">
    /// Clear screen after user press any key?. Defaults for <see cref="false"/>
    /// </param>
    public static void ShowMessage(string msg, bool clearAfter = false)
    {
        Console.Write(msg);
        Console.ReadLine();
        if (clearAfter) ConsoleUIApp.ClearScreen();
    }
}
