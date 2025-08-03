namespace ConsoleUI.Views.Extensions;

/// <summary>
/// General utility extension methods
/// </summary>
public static class Utilities
{
    /// <summary>
    /// Remeat a character for n number of times
    /// </summary>
    /// <param name="value">Character to repeat</param>
    /// <param name="times">Number of times to repeat the character</param>
    /// <returns></returns>
    public static string Repeat(this char value, int times)
    {
        return "".PadLeft(times, value);
    }
}
