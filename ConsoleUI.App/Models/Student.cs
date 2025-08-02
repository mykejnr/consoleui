using ConsoleUI.Views.Components;

namespace ConsoleUI.App.Models;

public class Student : ITableItem
{
    public int Id { get; set; }
    public string Surname { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public DateTime Birthday { get; set; }

    public void Display()
    {
        Console.WriteLine($"\tStudent Id: {Id}");
        Console.WriteLine($"\tSurname: {Surname}");
        Console.WriteLine($"\tFirstname: {FirstName}");
        Console.WriteLine($"\tAge: {DateTime.Today.Year - Birthday.Year}");
    }

    public override string ToString()
    {
        return $"{FirstName} {Surname}";
    }

    public List<string> ToTableRow()
    {
        return [Id.ToString(), Surname, FirstName, Birthday.ToString("dd MMM, yyyy")];
    }
}
