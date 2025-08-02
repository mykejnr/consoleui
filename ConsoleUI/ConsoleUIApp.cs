using ConsoleUI.Views.Extenstions;

namespace ConsoleUI;

public class ConsoleUIApp
{
    private static string? _appTitle;
    public static string AppTitle
    {
        get => _appTitle ?? "Console Application";
        set
        {
            _appTitle = value;
            _titleBar = null; // reset title bar to redraw it
        }
    }
    public static int Width { get; set; } = 80;

    private static string? _titleBar;

    public static void Start()
    {
        ClearScreen();
    }

    private static string DrawTitleBar()
    {
        var appTitle = $" {AppTitle.Trim()} "; // add left and right paddings
        appTitle = appTitle.AlignCenter(Width, '=');
        var bottomBorder = $"|{'_'.Repeat(Width - 2)}|";
        return $"{appTitle}\n{bottomBorder}\n";
    }

    public static void ClearScreen()
    {
        Console.Clear();
        _titleBar ??= DrawTitleBar();
        Console.WriteLine(_titleBar);
    }
}
