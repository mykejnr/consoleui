using ConsoleUI.Views.Extensions;

namespace ConsoleUI;

/// <summary>
/// Encapsulates general properties of the Console ui application
/// </summary>
public class ConsoleUIApp
{
    private static string? _appTitle;
    /// <summary>
    /// Application title. To rendered as first item on the console
    /// </summary>
    public static string AppTitle
    {
        get => _appTitle ?? "Console Application";
        set
        {
            _appTitle = value;
            _titleBar = null; // reset title bar to redraw it
        }
    }
    /// <summary>
    /// Get or set the width of the ui. Typically used when rendering the 
    /// title of the app
    /// </summary>
    public static int Width { get; set; } = 80;

    private static string? _titleBar;

    /// <summary>
    /// Initialize the application
    /// </summary>
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

    /// <summary>
    /// Clear the console screen, except the title bar
    /// </summary>
    public static void ClearScreen()
    {
        Console.Clear();
        _titleBar ??= DrawTitleBar();
        Console.WriteLine(_titleBar);
    }
}
