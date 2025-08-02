namespace ConsoleUI.Views.Components;

public class TableColumn
{
    public string Name { get; set; } = string.Empty;
    public ColumnAlign Align { get; set; } = ColumnAlign.Left;
    public int Width { get; set; }
}

public enum ColumnAlign
{
    Left,
    Right,
    Center,
}

public interface ITableItem
{
    List<string> ToTableRow();
}

