namespace ConsoleUI.Views.Components;

/// <summary>
/// Options for configuring <see cref="Menu"/>
/// </summary>
public class MenuBuilderOptions
{
    /// <summary>
    /// Paddings for the menu
    /// </summary>
    public Padding Padding { get; set; } = null!;
    /// <summary>
    /// Margins for the menu
    /// </summary>
    public Margin Margin { get; set; } = null!;
    /// <summary>
    /// Menu title
    /// </summary>
    public string Title { get; set; } = null!;
}
