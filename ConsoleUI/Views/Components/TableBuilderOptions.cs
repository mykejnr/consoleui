namespace ConsoleUI.Views.Components;

/// <summary>
/// Options for configuring a <see cref="TableView{T}"/>
/// </summary>
public class TableBuilderOptions
{
    /// <summary>
    /// Cell paddings
    /// </summary>
    public Padding CellPadding { get; set; } = null!;
    /// <summary>
    /// Table margins
    /// </summary>
    public Margin Margin { get; set; } = null!;
    /// <summary>
    /// Table titles
    /// </summary>
    public string Title { get; set; } = null!;
    /// <summary>
    /// Table column names
    /// </summary>
    public List<string> Columns { get; set; } = [];
}
