using ConsoleUI.Views.Components;

namespace ConsoleUI.App.Models;

public class Course : ITableItem
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Code { get; set; } = null!;
    public int CreditHours { get; set; }

    public override string ToString()
    {
        return $"[{Code}] {Title}";
    }

    public List<string> ToTableRow()
    {
        return [Id.ToString(), Code, Title, CreditHours.ToString()];
    }
}
