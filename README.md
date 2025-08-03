# CONSOLE UI 
A SIMPLE VIEW LIBRARY FOR CONSOLE APP

## Installation

```powershell
dotnet add package ConsoleUI
```

## Usage
```csharp
using ConsoleUI;
ConsoleApp.Run(args, app =>
{
	app.AddPage("Home", page =>
	{
		page.AddText("Welcome to ConsoleUI!");
		page.AddButton("Click Me", () => Console.WriteLine("Button Clicked!"));
	});
	app.AddPage("Settings", page =>
	{
		page.AddText("Settings Page");
		page.AddButton("Back", () => app.GoBack());
	});
});
```

## License
MIT License