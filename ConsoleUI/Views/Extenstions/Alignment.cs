namespace ConsoleUI.Views.Extenstions;

public static class Alignment
{
    public static string AlignLeft(this string value, int width)
    {
        return value.PadRight(width);
    }
    public static string AlignRight(this string value, int width)
    {
        return value.PadLeft(width);
    }

    public static string AlignCenter(this string value, int width, char? fill = null)
    {
        fill ??= ' ';
        int padding = (width - value.Length) / 2;
        return "".PadLeft(padding, fill.Value) + value.PadRight(width - padding, fill.Value);
    }
}
