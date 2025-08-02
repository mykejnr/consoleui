using ConsoleUI.Views;
using ConsoleUI.Views.Components;
using ConsoleUI.IO;
using ConsoleUI.App.Models;

namespace ConsoleUI.App.Controllers;

/// <summary>
/// Class responsible for managing (add, update, view) students
/// </summary>
public class StudentController(DataContext dContext, CourseController courses, AssessmentController assessments)
{
    /// <summary>
    /// Create and return a student
    /// </summary>
    /// <param name="clearScreen">
    /// Clear screen before taking user input?
    /// </param>
    /// <returns>New created <see cref="Student"/></returns>
    public Student? CreateStudent(bool clearScreen = true)
    {
        if (clearScreen)
        {
            ConsoleUIApp.ClearScreen();
            ConsoleHelper.PrintHeading("Add New Student");
        }

        var student = new Student();

        try
        {
            student.Surname = ConsoleReader.GetInput("Surname");
            student.FirstName = ConsoleReader.GetInput("Firstname");
            student.Birthday = ConsoleReader.GetDate("Date of Birth");
        }
        catch (ConsoleReaderAborted)
        {
            ConsoleHelper.ShowMessage("Operation Cancelled. Field is required...");
            return null;
        }
        catch (ConsoleReaderInvalidInput ex)
        {
            ConsoleHelper.ShowMessage($"\n{ex.Message}\nOperation Cancelled...");
            return null;
        }

        student.Id = dContext.Students.Count + 1;

        dContext.Students.Add(student);
        dContext.SaveChanges();
        ConsoleHelper.ShowMessage($"\nStudent(id={student.Id}) saved sucessfully. ");

        return student;
    }

    /// <summary>
    /// Gives the user the chance to create many students continuesly 
    /// until they opt out
    /// </summary>
    public void CreateStudents()
    {
        bool createAnotherStudent = false;
        do
        {
            CreateStudent(!createAnotherStudent);
            createAnotherStudent = ConsoleReader.GetBool("Add another student?");
        } while (createAnotherStudent);
    }

    /// <summary>
    /// Displays a table and return the selected <see cref="Student"/>
    /// from the table by the user
    /// </summary>
    /// <returns>
    /// Returns the selected <see cref="Student"/>, or null if not 
    /// student is selected from the table
    /// </returns>
    public Student? GetStudent()
    {
        var option = DrawStudentTable();
        if (option == -1) return null;
        return dContext.Students[option];
    }

    /// <summary>
    /// Find <see cref="Student"/> by student id provided by the user on the prompt
    /// </summary>
    /// <returns></returns>
    public Student? FindStudent()
    {
        int studentId;
        var msg = "";
        var attempts = 3;
        Student? student = null;

        ConsoleUIApp.ClearScreen();
        ConsoleHelper.PrintHeading("Find Student by ID");

        do
        {
            try
            {
                studentId = ConsoleReader
                    .GetInt("Enter student Id" + msg, required: false, defaultValue: -1);
                if (studentId == -1) break;// no input (default value)
            }
            catch (ConsoleReaderInvalidInput)
            {
                msg = " (INVALID INPUT)!!";
                continue;
            }

            student = dContext.Students.Find(s => s.Id == studentId);
            msg = student == null ? " (STUDENT NOT FOUND)!!" : "";
            attempts--;
        }
        // Give the user three attempts to provide valid, existing user id
        while (student is null && attempts > 0);

        // Allow the user to update the "found" user
        if (student != null) return UpdateStudent(student);

        ConsoleHelper.ShowMessage("\nOperation cancelled... ");
        return null;
    }

    /// <summary>
    /// Update <paramref name="student"/> information
    /// </summary>
    /// <param name="student"></param>
    /// <returns></returns>
    public Student UpdateStudent(Student student)
    {
        ConsoleUIApp.ClearScreen();
        ConsoleHelper.PrintHeading(
            $"Updating student: {student}", "Leave prompt blank to keep field unchanged.");

        student.FirstName = ConsoleReader.GetInput(
            $"Firstname ({student.FirstName})",
            required: false,
            defaultValue: student.FirstName
        );

        student.Surname = ConsoleReader.GetInput(
            $"Surname ({student.Surname})",
            required: false,
            defaultValue: student.Surname
        );

        try
        {
            var dateString = student.Birthday.ToString("yyyy-MM-dd");
            student.Birthday = ConsoleReader.GetDate(
                $"Date of birth({dateString})",
                required: false,
                defaultValue: student.Birthday
            );
        }
        catch (ConsoleReaderInvalidInput ex)
        {
            ConsoleHelper.ShowMessage($"\n{ex.Message}\nOperation Cancelled... ");
        }

        dContext.SaveChanges();
        ConsoleHelper.ShowMessage("\nStudent updated successfully... ");

        return student;
    }

    /// <summary>
    /// Allow user to enter marks (register a course) for a user
    /// they selected from a list of users
    /// </summary>
    /// <returns>
    /// New created <see cref="StudentMark"/>
    /// </returns>
    public StudentMark? EnterStudentMark()
    {
        var student = GetStudent();
        if (student == null) return null;
        return EnterStudentMark(student);
    }
    
    /// <summary>
    /// Enter marks (register) a course for the provided <paramref name="student"/>
    /// </summary>
    /// <param name="student"></param>
    /// <returns>
    /// New created <see cref="StudentMark"/>
    /// </returns>
    public StudentMark? EnterStudentMark(Student student)
    {
        var course = courses.GetCourse(student, newCourse: true);
        if (course == null) return null;
        return assessments.EnterMarks(student, course);
    }

    /// <summary>
    /// Allower user to update marks for a course registered by <paramref name="student"/>
    /// <para>
    /// User is allowed to select a course from a list
    /// of courses the user has already registered for
    /// </para>
    /// </summary>
    /// <param name="student"></param>
    /// <returns></returns>
    public StudentMark? UpdateStudentMark(Student student)
    {
        var course = assessments.DisplayStudentScoreSheet(student);
        if (course == null) return null;
        return assessments.UpdateMarks(student, course);
    }

    private int DrawStudentTable()
    {
        var tableOptions = new TableBuilderOptions
        {
            Columns = ["Id", "Surname", "Other Names", "Date of Birth"],
        };
        var table = new TableView<Student>(dContext.Students, tableOptions);
        return table.Render();
    }

    private Menu? _subMenu;
    public void ListStudents()
    {
        var option = DrawStudentTable();

        if (option == -1) return;
        var student = dContext.Students[option];

        _subMenu ??= CreateSubMenu();

        while (option != -1)
        {
            option = _subMenu.Render($"Student: {student}");
            switch (option)
            {
                case 0:
                    UpdateStudent(student);
                    break;
                case 1:
                    EnterStudentMark(student);
                    break;
                case 2:
                    UpdateStudentMark(student);
                    break;
            }
        }
    }

    private static Menu CreateSubMenu()
    {
        List<MenuItem> list = [
            new() { Name = "Edit" },
            new() { Name = "Enter marks" },
            new() { Name = "Score sheet" },
        ];

        return new Menu(list, new());
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
                Name = "List All Student",
                Execute = ListStudents,
            },
            new() {
                Name = "Add New Student",
                Execute = () => CreateStudents(),
            },
            new() {
                Name = "Find Student",
                Execute = () => FindStudent()
            },
        ];

        return new Menu(list, new() { Title = "Students Portal"});
    }
}
