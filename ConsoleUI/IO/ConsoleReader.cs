namespace ConsoleUI.IO;

/// <summary>
/// Contain static methods for reading from the console and 
/// converting to a preferred type
/// </summary>
public class ConsoleReader
{
    /// <summary>
    /// Read a string from the standard input.
    /// </summary>
    /// <param name="caption">Caption to be shown to the usser, on the input prompt</param>
    /// <param name="attempts">
    /// Gives the user this number of attempts to entera valid input
    /// </param>
    /// <param name="required">
    /// Disallow blank input? if true, a <see cref="ConsoleReaderAborted"/> exception
    /// will be raised if nothing is entered by the user
    /// </param>
    /// <param name="defaultValue">
    /// Value to return if the user enteres nothing on the prompt (and only if
    /// required = false)
    /// </param>
    /// <returns></returns>
    /// <exception cref="ConsoleReaderAborted"></exception>
    public static string GetInput(string caption, int attempts = 3, bool required = true, string defaultValue = "")
    {
        string value;
        caption = required ? "*" + caption : caption;

        do
        {
            Console.Write($"{caption}: ");
            value = Console.ReadLine()!;
            attempts--;
        }
        while (attempts > 0 && value == string.Empty && required);

        // After we've exhausted all the attempts, but haven't gotten
        // any value from the user
        if (value.Trim() == string.Empty)
        {
            if (required)
                //,throw an error if this input is required
                throw new ConsoleReaderAborted("Input operation aborted");
            return defaultValue;
        }

        return value.Trim();
    }

    /// <summary>
    /// Read an integer from the standard input
    /// </summary>
    /// <param name="caption">Caption to be shown to the usser, on the input prompt</param>
    /// <param name="attempts">
    /// Gives the user this number of attempts to entera valid input
    /// </param>
    /// <param name="required">
    /// Disallow blank input? if true, a <see cref="ConsoleReaderAborted"/> exception
    /// will be raised if nothing is entered by the user
    /// </param>
    /// <param name="defaultValue">
    /// Value to return if the user enteres nothing on the prompt (and only if
    /// required = false)
    /// </param>
    /// <returns></returns>
    /// <exception cref="ConsoleReaderAborted"></exception>
    /// <exception cref="ConsoleReaderInvalidInput"></exception>
    public static int GetInt(string caption, int attempts = 3, bool required = true, int defaultValue = 0)
    {
        var value = GetInput(caption, attempts, required);
        if (value == string.Empty) return defaultValue;

        var isInt = int.TryParse(value, out int intValue);
        if (!isInt)
            throw new ConsoleReaderInvalidInput($"Invalid input({value}). Expected an integer value");

        return intValue;
    }

    /// <summary>
    /// Read a date value from the standard input
    /// </summary>
    /// <param name="caption">Caption to be shown to the usser, on the input prompt</param>
    /// <param name="attempts">
    /// Gives the user this number of attempts to entera valid input
    /// </param>
    /// <param name="required">
    /// Disallow blank input? if true, a <see cref="ConsoleReaderAborted"/> exception
    /// will be raised if nothing is entered by the user
    /// </param>
    /// <param name="defaultValue">
    /// Value to return if the user enteres nothing on the prompt (and only if
    /// required = false)
    /// </param>
    /// <returns></returns>
    /// <exception cref="ConsoleReaderAborted"></exception>
    /// <exception cref="ConsoleReaderInvalidInput"></exception>
    public static DateTime GetDate(string caption, int attempts = 3, bool required = true, DateTime defaultValue = default)
    {
        caption += " (yyyy-mm-dd)";
        var dateString = GetInput(caption, attempts, required);


        if (dateString == string.Empty) return defaultValue;

        var OK = DateTime.TryParse(dateString, out DateTime value);
        if (!OK)
            throw new ConsoleReaderInvalidInput("Invalid date format");

        return value;
    }

    /// <summary>
    /// Read a float value from the standard input
    /// </summary>
    /// <param name="caption">Caption to be shown to the usser, on the input prompt</param>
    /// <param name="attempts">
    /// Gives the user this number of attempts to enter a valid input
    /// </param>
    /// <param name="required">
    /// Disallow blank input. if true, a <see cref="ConsoleReaderAborted"/> exception
    /// will be raised if nothing is entered by the user
    /// </param>
    /// <param name="defaultValue">
    /// Value to return if the user enters nothing on the prompt (and only if
    /// required = false)
    /// </param>
    /// <returns></returns>
    /// <exception cref="ConsoleReaderAborted"></exception>
    /// <exception cref="ConsoleReaderInvalidInput"></exception>
    public static float GetFloat(string caption, int attempts = 3, bool required = true, float defaultValue = 0)
    {
        var value = GetInput(caption, attempts, required);
        if (value == string.Empty) return defaultValue;

        var isFloat = float.TryParse(value, out float fvalue);
        if (!isFloat)
            throw new ConsoleReaderInvalidInput($"Invalid input({value}). Expected a decimal number");

        return fvalue;
    }

    /// <summary>
    /// Get a yes / no respose from the user from the standard input
    /// <para>
    /// Any imput that starts with "y" will be interperated as "yes", hence
    /// returning true, else we return false
    /// </para>
    /// </summary>
    /// <param name="caption">
    /// Caption or question to be shown on the input prompt
    /// </param>
    /// <returns></returns>
    public static bool GetBool(string caption)
    {
        caption += " (y/n)";

        var value = GetInput(caption, required: false);
        if (value == null || value.Trim().Length == 0)
            return false;

        return value.StartsWith("y", StringComparison.CurrentCultureIgnoreCase);
    }
}


/// <summary>
/// Raised when a user enters nothing on the input
/// </summary>
/// <param name="msg"></param>
public class ConsoleReaderAborted(string msg) : Exception(msg) { }

/// <summary>
/// Raised for invalid imput by a user
/// </summary>
/// <param name="msg"></param>
public class ConsoleReaderInvalidInput(string msg) : Exception(msg) { }
