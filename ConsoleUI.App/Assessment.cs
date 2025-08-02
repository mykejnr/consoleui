using ConsoleUI.Views;
using ConsoleUI.Views.Components;
using ConsoleUI.App.Models;
using ConsoleUI.App.Controllers;

namespace ConsoleUI.App;

/// <summary>
/// Main application class. Displays main menu for the app
/// </summary>
public class Assessment
{
    private readonly StudentController _students;
    private readonly AssessmentController _assessments;
    private readonly CourseController _courses;
    public Assessment(DataContext dContext)
    {
        _courses = new CourseController(dContext);
        _assessments = new AssessmentController(dContext, _courses);
        _students = new StudentController(dContext, _courses, _assessments);
    }

    private bool _quit = false;
    private Menu? _mainMenu;
    public void Start()
    {
        // Construct the main application menu.
        // the null coalescing operator "??=", ensures that
        // the menu is only constructed onces.
        // i.e if _mainMenu is not null, then construct it
        // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-coalescing-operator
        _mainMenu ??= CreateMainMenu();

        int option = 0;

        // Start program loop
        // This ensures the program runs until the usser issues
        // a "quit" command
        while ( !(option == -1 || _quit) )
            option = _mainMenu.Render();

        // If we are here (outside the while loop) then the
        // user issued a "quit" command
        // Or, the user closed the main menu without selecing
        // any of the menu options, hence ( option == -1)
        ShowGoodByMsg(option == -1);
    }

    /// <summary>
    /// Construct and return a <seealso cref="Menu"/> object
    /// </summary>
    private Menu CreateMainMenu()
    {
        List<MenuItem> items = [
            new() {
                Name = "Course sheet",
                Execute = _assessments.DisplayCourseSheet
            },
            new() {
                Name = "Enter Student Marks",
                Execute = () => _students.EnterStudentMark()
            },
            new() {
                Name = "Add New Student",
                Execute = () => _students.CreateStudent()
            },
            new() {
                Name = "Add New Course",
                Execute = () => _courses.CreateCourse()
            },
            new() { Name = "-" },
            new() {
                Name = "Student Portal",
                Execute = _students.Portal
            },
            new() {
                Name = "Courses",
                Execute = _courses.Portal
            },
            new() { Name = "-" },
            new() {
                Name = "Quit",
                Execute = () => _quit = true
            },
        ];

        return new Menu(items, new() { Title = "Main Menu" });
    }

    private static void ShowGoodByMsg(bool noOption = false)
    {
        Console.WriteLine(); // empty line
        Console.WriteLine("\n****************************************\n");
        if (noOption)
        {
            Console.WriteLine("\tNo menu selected");
        }
        Console.WriteLine("\tQuiting application.");
        Console.Write("\n*************** Good Bye ***************\n");
        Console.Read();
    }
}
