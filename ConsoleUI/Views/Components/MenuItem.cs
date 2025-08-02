namespace ConsoleUI.Views.Components;

public class MenuItem()
{
    public string Name { get; set; } = string.Empty;
    public Action Execute { get; set; } = () => { };
}
