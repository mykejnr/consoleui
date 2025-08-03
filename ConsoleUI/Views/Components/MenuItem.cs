namespace ConsoleUI.Views.Components;

/// <summary>
/// An item rendered on a <see cref="Menu"/>
/// </summary>
public class MenuItem()
{
    /// <summary>
    /// Name of the item to be rendered on the menu.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Action to execute when the menu item is invoked
    /// </summary>
    public Action Execute { get; set; } = () => { };
}
