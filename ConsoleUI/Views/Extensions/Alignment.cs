namespace ConsoleUI.Views.Extensions;

/// <summary>
/// Extension methods for aligning strings
/// </summary>
public static class Alignment
{
    /// <summary>
    /// Align string to the left.<br/>
    /// Pads the string with white spaces
    /// </summary>
    /// <param name="value">The string value to be aligned</param>
    /// <param name="width">Total width of the string including the padded spaces</param>
    /// <returns></returns>
    public static string AlignLeft(this string value, int width)
    {
        return value.PadRight(width);
    }

    /// <summary>
    /// Align string to the right.<br/>
    /// Pads the string with white spaces
    /// </summary>
    /// <param name="value">The string value to be aligned</param>
    /// <param name="width">Total width of the string including the padded spaces</param>
    /// <returns></returns>
    public static string AlignRight(this string value, int width)
    {
        return value.PadLeft(width);
    }

    /// <summary>
    /// Centers a string within the provided width.<br/>
    /// Pads the remaining characters with an empty space
    /// </summary>
    /// <param name="value"></param>
    /// <param name="width"></param>
    /// <param name="fill"></param>
    /// <returns></returns>
    public static string AlignCenter(this string value, int width, char? fill = null)
    {
        fill ??= ' ';
        int padding = (width - value.Length) / 2;
        return "".PadLeft(padding, fill.Value) + value.PadRight(width - padding, fill.Value);
    }
}
