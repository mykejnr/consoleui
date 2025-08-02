namespace ConsoleUI.Views.Extenstions;

public static class Utilities
{
    public static string Repeat(this char value, int times)
    {
        return "".PadLeft(times, value);
    }
}
