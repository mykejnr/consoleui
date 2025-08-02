using ConsoleUI.Views;
using ConsoleUI.Views.Components;
using ConsoleUI.IO;

using ConsoleUI.App.Models;

namespace ConsoleUI.App.Controllers;

/// <summary>
/// Class responsible for managing (add, update, view) courses
/// </summary>
public class CourseController(DataContext dContext)
{
    /// <summary>
    /// Create and return a new <see cref="Course"/>
    /// </summary>
    /// <param name="clearScreen">
    /// Clear screen before asking for user inputs?
    /// </param>
    public Course? CreateCourse(bool clearScreen = true)
    {
        if(clearScreen)
        {
            ConsoleUIApp.ClearScreen();
            ConsoleHelper.PrintHeading("Create New Course");
        }

        var course = new Course();

        try
        {
            course.Code = GetCourseCode()!;
            course.Title = ConsoleReader.GetInput("Course Title");
            course.CreditHours = ConsoleReader.GetInt("Credit Hours");
        }
        catch (ConsoleReaderAborted ex)
        {
            ConsoleHelper.ShowMessage($"\n{ex.Message}...");
            return null;
        }
        catch (ConsoleReaderInvalidInput ex)
        {
            ConsoleHelper.ShowMessage($"\n{ex.Message}...");
            return null;
        }

        course.Id = dContext.Courses.Count + 1;
        dContext.Courses.Add(course);
        dContext.SaveChanges();
        ConsoleHelper.ShowMessage($"\nCourse(code: {course.Code}) Created... ");

        return course;
    }

    /// <summary>
    /// Gives the user the chance to create more course continuesly 
    /// until they opt out
    /// </summary>
    public void CreateCourses()
    {
        bool createAnother = false;
        do
        {
            CreateCourse(!createAnother);
            createAnother = ConsoleReader.GetBool("Add another course?");
        } while (createAnother);
    }

    /// <summary>
    /// Update the details of an existing course
    /// </summary>
    /// <param name="course"></param>
    /// <returns></returns>
    public Course? UpdateCourse(Course course)
    {
        ConsoleUIApp.ClearScreen();
        ConsoleHelper.PrintHeading($"Update Course {course}",
            "Leave prompt blank to keep field unchanged.");

        try
        {
            course.Code = GetCourseCode(defaultValue: course.Code);
            course.Title = ConsoleReader.GetInput(
                $"Course Title ({course.Title})", required: false, defaultValue: course.Title);
            course.CreditHours = ConsoleReader.GetInt
                ($"Credit Hours ({course.CreditHours})", required: false, defaultValue: course.CreditHours);
        }
        catch (ConsoleReaderInvalidInput ex)
        {
            ConsoleHelper.ShowMessage($"\n{ex.Message}...");
            return null;
        }

        dContext.SaveChanges();
        ConsoleHelper.ShowMessage($"\nCourse(code: {course.Code}) Updated... ");

        return course;
    }

    /// <summary>
    /// Helper method to get and validate course code from
    /// the user when creating or updating a course
    /// </summary>
    /// <param name="defaultValue">
    /// Default value to return incase the user enters nothing
    /// on the prompt.
    /// <para>
    /// If not provided, <see cref="ConsoleReaderAborted"/> will be thrown if
    /// the user provides no input
    /// </para>
    /// </param>
    /// <returns>The course code entered by the user</returns>
    /// <exception cref="ConsoleReaderAborted"></exception>
    private string GetCourseCode(string? defaultValue = null)
    {
        int attempts = 3;
        var required = defaultValue == null;
        var prompt = defaultValue == null ? "Course Code" : $"Course Code {defaultValue}";
        var msg = "";

        // Give the user 3 attempts to entered a valid course code
        // that does not already exist
        while (attempts > 0)
        {
            var code = ConsoleReader.GetInput(
                prompt + msg,
                required: required, defaultValue: defaultValue ?? "");

            // If the value entered by the user is same as the default value
            // do not border validating whether the code already exist. just
            // return the course code
            if (code == defaultValue) return code;

            // Return value if a course with the same code does not
            // already exist
            if (!dContext.Courses.Any(c => c.Code == code)) return code;

            msg = " (COURSE CODE ALREAY EXIST!!)";
        }

        // If we are here, the the use couldn't entere a valid course code.
        // So return default value if it was provided...
        if (defaultValue is not null) return defaultValue;

        // ... else throw an error, let the caller deal with it
        throw new ConsoleReaderAborted("Operation cancelled.");
    }

    /// <summary>
    /// Display all courses in a table view
    /// </summary>
    public void ListCourses()
    {
        while (true)
        {
            var option = DrawCourseTable();

            // Table is closed without selecting any course
            if (option == -1) return;

            // Update the selected course from the table
            var student = dContext.Courses[option];
            UpdateCourse(student);
        }
    }

    /// <summary>
    /// Displays a list of all courses and return the selected
    /// course from the table by the user.
    /// </summary>
    /// <param name="caption">
    /// Caption to displayt on the table
    /// </param>
    /// <returns>
    /// Selected <see cref="Course"/>, null if table
    /// is closed without selecting any course
    /// </returns>
    public Course? GetCourse(string caption)
    {
        var option = DrawCourseTable(title: caption);
        if (option == -1) return null;
        return dContext.Courses[option];
    }

    /// <summary>
    /// Displays a list of courses (registered/not registered) for a
    /// student and return the selected course from the table by the user
    /// </summary>
    /// <param name="student"></param>
    /// <param name="newCourse">
    /// If <see cref="true"/>, display courses that are not registered by
    /// the provided student, else displays courses that are registered by the
    /// provided student
    /// </param>
    /// <returns>
    /// The selected <see cref="Course"/> from the table, or null
    /// if no course is selected from the table
    /// </returns>
    public Course? GetCourse(Student student, bool newCourse = false)
    {
        string title;
        var courses = dContext.Courses;
        var marks = dContext.StudentMarks
            .Where(m => m.StudentId == student.Id).ToList();

        if (newCourse)
        {
            title = $"Select new course for: {student}";
            courses = courses.Where(c => !marks.Any(m => m.CourseCode == c.Code))
                .ToList();
        }
        else
        {
            title = $"Registered courses for: {student}";
            courses = courses.Where(c => marks.Any(m => m.CourseCode == c.Code))
                .ToList();
        }

        var option = DrawCourseTable(courses, title);
        if (option == -1) return null;
        return courses[option];
    }

    private int DrawCourseTable(List<Course>? cources = null, string title = "All Courses")
    {
        cources ??= dContext.Courses;

        var tableOptions = new TableBuilderOptions
        {
            Title = title,
            Columns = ["Id", "Code", "Title", "Credit Hours"],
        };
        var table = new TableView<Course>(cources, tableOptions);
        table.Align(3, ColumnAlign.Right).Align(1, ColumnAlign.Center);
        return table.Render();
    }

    private Menu? _menu = null;
    public void Portal()
    {
        _menu ??= CreateMenu();
        int option = 0;

        while (option >= 0)
            option = _menu.Render();
    }

    private Menu CreateMenu()
    {
        List<MenuItem> list = [
            new() {
                Name = "List All Courses",
                Execute = () => ListCourses()
            },
            new() {
                Name = "Add New Course",
                Execute = () => CreateCourses()
            }
        ];

        return new Menu(list, new() { Title = "Courses Portal" });
    }
}
