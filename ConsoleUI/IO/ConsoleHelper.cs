namespace ConsoleUI.IO;

/// <summary>
/// Host utility methods for writing to the console
/// </summary>
public class ConsoleHelper
{
    /// <summary>
    /// Draw a sigle underline using the '-' character
    /// </summary>
    /// <param name="width">Number of characters to dray</param>
    public static void SingleUnderline(int width = 50)
    {
        Console.WriteLine("".PadRight(width, '-'));
    }

    /// <summary>
    /// Draw a double underline using the '=' character.
    /// </summary>
    /// <param name="width">Number of characters to draw</param>
    public static void DoubleUnderline(int width = 50)
    {
        Console.WriteLine("".PadRight(width, '='));
    }

    /// <summary>
    /// Write a heading with a single underline
    /// </summary>
    /// <param name="title">Main title to write</param>
    /// <param name="subTitles">Oprional subtitles</param>
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
    /// If true, clears the screen after user presse enter
    /// </param>
    public static void ShowMessage(string msg, bool clearAfter = false)
    {
        Console.Write(msg);
        Console.ReadLine();
        if (clearAfter) ConsoleUIApp.ClearScreen();
    }
}
