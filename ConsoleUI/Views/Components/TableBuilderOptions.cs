namespace ConsoleUI.Views.Components;

public class TableBuilderOptions
{
    public Padding CellPadding { get; set; } = null!;
    public Margin Margin { get; set; } = null!;
    public string Title { get; set; } = null!;
    public List<string> Columns { get; set; } = [];
}
