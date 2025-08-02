using ConsoleUI.App;
using ConsoleUI.App.Models;
using ConsoleUI;

// Draw a text that displays our application name in a "title bar".
// Let's make our app look stylish and professional!!!
ConsoleUIApp.AppTitle = "Continues Assessment System";
ConsoleUIApp.Width = 80; // set console width
ConsoleUIApp.Start(); 

// We want to save our application data (database) in the
// user's home directory. eg: "C:\Users\Dennisia".
// The line below asks the operating system (os) to give
// us the path to the user's home directory
var homedir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

// Then let's create a folder in the user's home directory
var appdir = Path.Combine(homedir!, "cassapp");
Directory.CreateDirectory(appdir); // create directory in not exist

// Now we create a file in the directory "cassapp" that we
// just created above, (only if we haven't created it already.
// This file is where we dumb the application data
var filename = Path.Combine(appdir, "cass.json");
if (!File.Exists(filename))
    File.Create(filename).Close();

// Initialize the class responsible for handling the database
var dataContext = new DataContext(filename);

// Initialize the main app
var app = new Assessment(dataContext);

// Start the app
app.Start();
