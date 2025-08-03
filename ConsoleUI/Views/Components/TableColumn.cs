namespace ConsoleUI.Views.Components;

/// <summary>
/// Encapsulate a table column properties
/// </summary>
public class TableColumn
{
    /// <summary>
    /// Name of the table column
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Alignment of the table (left, right, center)
    /// </summary>
    public ColumnAlign Align { get; set; } = ColumnAlign.Left;
    /// <summary>
    /// Width of the table column
    /// </summary>
    public int Width { get; set; }
}

/// <summary>
/// Alignment of a table column
/// </summary>
public enum ColumnAlign
{
    /// <summary>
    /// Align column items to the left
    /// </summary>
    Left,
    /// <summary>
    /// Align column items to the right
    /// </summary>
    Right,
    /// <summary>
    /// Centers column items
    /// </summary>
    Center,
}

/// <summary>
/// Classes need to implement this interface to enable them to be passed
/// to a <see cref="TableView{T}"/> as table items
/// </summary>
public interface ITableItem
{
    /// <summary>
    /// Return a list of string that represent each cell in the row
    /// </summary>
    /// <returns></returns>
    List<string> ToTableRow();
}

