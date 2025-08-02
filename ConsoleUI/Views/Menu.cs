using ConsoleUI.IO;
using ConsoleUI.Views.Components;
using ConsoleUI.Views.Extenstions;

namespace ConsoleUI.Views;

/// <summary>
/// Class to render a menu and execute selected menu actions
/// </summary>
/// <param name="menuItems"></param>
/// <param name="title"></param>
public class Menu
{
    private int _width;
    private readonly List<MenuItem> _menuItems;
    // Extract the action menu items that are not seperators ("-")
    private readonly List<MenuItem> _actionMenus;
    MenuBuilderOptions _builderOptions;

    private readonly string MARGIN_LEFT_STR;

    public MenuBuilderOptions BO => _builderOptions;

    public Menu(List<MenuItem> menuItems, MenuBuilderOptions builderOptions)
    {
        _builderOptions = builderOptions;
        SetDefaultBuilderOptions();

        _menuItems = menuItems;
        _width = ComputeWidth();
        _actionMenus = menuItems.Where(x => x.Name != "-").ToList();

        MARGIN_LEFT_STR = ' '.Repeat(_builderOptions.Margin.Left);
    }

    private void SetDefaultBuilderOptions()
    {
        _builderOptions.Title ??= "Untittled Menu";
        _builderOptions.Margin ??= new() { Left = 4 }; // 4 spaces to the left by default
        _builderOptions.Padding ??= new()
        {
            Left = 2,
            Right = 2,
        };
    }

    /// <summary>
    /// Draws the menu with it items on the screen
    /// </summary>
    /// <param name="newTitle">
    /// Replace the title provided when constructing the menu
    /// </param>
    /// <returns>
    /// The index position of the selected menu item
    /// <para>
    /// NOTE: The indexing is "zero based" and excludes the any number 
    /// of seperators ("-")
    /// </para>
    /// </returns>
    public int Render(string? newTitle = null)
    {
        if (newTitle != null)
        {
            _builderOptions.Title = newTitle;
            _width = ComputeWidth();
        }

        ConsoleUIApp.ClearScreen();

        // Save cursor positions, we will reset cursor position
        // on each key down/up press
        var left = Console.CursorLeft;
        var top = Console.CursorTop;
        var selection = 0;

        while (true)
        {
            Console.SetCursorPosition(left, top);
            DrawMenu(selection);

            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.Enter:
                    ExecuteMenuAction(selection);
                    return selection;
                case ConsoleKey.DownArrow:
                    selection += 1;
                    if (selection >= _actionMenus.Count)
                        selection = 0;
                    break;
                case ConsoleKey.UpArrow:
                    selection -= 1;
                    selection = Math.Max(0, selection);
                    break;
                case ConsoleKey.X:
                    return -1;
            }
        }
    }

    private void DrawMenu(int selection)
    {
        var border = CreateBorder(); // Create (not draw) a horizontal border

        Console.WriteLine(border); // Draw top border
        DisplayTitle(); // Draw title
        Console.WriteLine(border); // Draw border below title

        // Items may contain "-" seperators, let's keep track 
        // of our own index of (actual menu items)
        int itemAt = 0;

        foreach (var item in _menuItems)
        {
            if (item.Name == "-") // Menu Seperator
            {
                Console.WriteLine(border);
            }
            else
            {
                DrawMenuItem(item, itemAt == selection);
                itemAt += 1;
            }
        }

        Console.WriteLine(border); // draw bottom menu border
        Console.Write("\n  Press [x] to close menu.  "); // empty line
    }

    private void DrawMenuItem(MenuItem item, bool selected)
    {
        var value = item.Name.PadRight(_width - BO.Padding.Left);
        value = $"{MARGIN_LEFT_STR}|{' '.Repeat(BO.Padding.Left)}{value}|";

        if (selected)
        {
            value += " <"; // selected row indicator
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(value);
            Console.ForegroundColor = defaultColor;
        }
        else
        {
            value += "   "; // Clean the previous " <" that was drawn
            Console.WriteLine(value);
        }
    }

    private void DisplayTitle()
    {
        var title = _builderOptions.Title.AlignCenter(_width);
        Console.WriteLine($"{MARGIN_LEFT_STR}|{title}|");
    }

    private string CreateBorder()
    {
        return MARGIN_LEFT_STR + "+" + "".PadRight(_width, '-') + "+";
    }

    public int ComputeWidth()
    {
        // The width of the menu is the menu item with longest name,
        // or the width of title, if it is longer thatn all items in the menu
        var width = Math.Max(
            _builderOptions.Title.Length,
            _menuItems.Max(m => m.Name.Length)
        );
        return width + _builderOptions.Padding.Left + _builderOptions.Padding.Right;
    }

    private void ExecuteMenuAction(int option)
    {
        _actionMenus[option].Execute();
    }
}
