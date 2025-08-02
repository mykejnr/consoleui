using ConsoleUI.IO;
using ConsoleUI.Views;
using ConsoleUI.App.ViewModels;
using ConsoleUI.App.Models;
using ConsoleUI.Views.Components;

namespace ConsoleUI.App.Controllers;


/// <summary>
/// Class responsible for managing (adding, updating, displaying marks
/// for registered courses of students
/// </summary>
/// <param name="dContext"></param>
/// <param name="courses"></param>
public class AssessmentController(DataContext dContext, CourseController courses)
{
    /// <summary>
    /// Enter marks (Register a new course) for a student
    /// </summary>
    /// <param name="student">Student we are entering the marks for</param>
    /// <param name="course">The course to register</param>
    /// <returns>
    /// The newly created mark entry of type <see cref="StudentMark"/>
    /// or null if there was an error or the user aborted entry.
    /// <para>A message will be displayed if operation wasn't successfull</para>
    /// </returns>
    public StudentMark? EnterMarks(Student student, Course course)
    {
        ConsoleUIApp.ClearScreen();
        ConsoleHelper.PrintHeading(
            "Enter Marks For Student:",
            $"\tStudent: {student}",
            $"\tCourse: {course.Title}"
         );

        var mark = new StudentMark
        {
            StudentId = student.Id,
            CourseCode = course.Code
        };

        try
        {
            mark.Attendance = GetMark("Attendance (5%)", 5);
            mark.Assignment = GetMark("Assignment (25%)", 25);
            mark.EndOfTerm = GetMark("Exams Score (70%)", 70);
        }
        catch (ConsoleReaderInvalidInput ex)
        {
            ConsoleHelper.ShowMessage($"\n{ex.Message}... ");
            return null;
        }
        catch (ConsoleReaderAborted)
        {
            ConsoleHelper.ShowMessage($"\nOperation Cancelled... ");
            return null;
        }

        mark.Id = dContext.Students.Count + 1;
        dContext.StudentMarks.Add(mark);
        dContext.SaveChanges();

        ConsoleHelper.ShowMessage("\nNew marks entry added successfully...");
        return mark;
    }

    /// <summary>
    /// Update the scores of a student for a particular course
    /// </summary>
    /// <param name="student"></param>
    /// <param name="course"></param>
    /// <returns>The updated marks</returns>
    public StudentMark? UpdateMarks(Student student, Course course)
    {
        ConsoleUIApp.ClearScreen();
        ConsoleHelper.PrintHeading(
            $"Updating Marks For: {student}",
            $"\t{course}",
            "Leave prompt blank to keep field unchanged.");

        if (!EnsureStudentMark(student, course, out StudentMark mark))
            return null;

        try
        {
            mark.Attendance = GetMark(
                $"Attendance ({mark.Attendance} / 5%)", 5, mark.Attendance);
            mark.Assignment = GetMark(
                $"Assignment ({mark.Assignment} / 25%)", 25, mark.Assignment);
            mark.EndOfTerm = GetMark(
                $"Exams Score ({mark.EndOfTerm} / 70%)", 70, mark.EndOfTerm);
        }
        catch (ConsoleReaderInvalidInput ex)
        {
            ConsoleHelper.ShowMessage($"\n{ex.Message}");
            return null;
        }

        dContext.SaveChanges();
        ConsoleHelper.ShowMessage($"\nStudent mark updated... ");

        return mark;
    }

    /// <summary>
    /// Check whether the student has a score recorded for them
    /// for the course provided.
    /// </summary>
    /// <param name="student"></param>
    /// <param name="course"></param>
    /// <param name="mark">When mark is found, it is assigned to this 'out' parameter</param>
    /// <returns>
    /// True if the student has a mark recorded for them for the provided course
    /// else returns False
    /// </returns>
    private bool EnsureStudentMark(Student student, Course course, out StudentMark mark)
    {
        mark = dContext.StudentMarks.FirstOrDefault(m =>
            m.StudentId == student.Id && m.CourseCode == course.Code)!;

        if (mark != null) return true;

        // Student has no marks!. Ask the user to enter/register some
        // marks for the student

        Console.WriteLine($"\n{student} has not mark for {course}");
        try
        {
            if (ConsoleReader.GetBool($"Add mark for '{student.FirstName}'"))
                mark = EnterMarks(student, course)!;
        }
        catch (ConsoleReaderAborted) { }
        
        return false;
    }

    /// <summary>
    /// Helper function to receive an imput from the user as a student
    /// mark, and validate the input for the user
    /// </summary>
    /// <param name="caption">Caption to display to the user</param>
    /// <param name="max">The maximum number required for this mark</param>
    /// <param name="defaultValue">
    /// When the user enters no value, the value will be returned
    /// </param>
    /// <returns></returns>
    /// <exception cref="ConsoleReaderInvalidInput"></exception>
    private static int GetMark(string caption, int max, int? defaultValue = null)
    {
        var required = defaultValue == null;
        var value = ConsoleReader
            .GetInt(caption, required: required, defaultValue: defaultValue ?? 0);

        if (value < 0 || value > max)
            throw new ConsoleReaderInvalidInput($"Value cannot be negative or greater than {max}");

        return value;
    }

    /// <summary>
    /// Display the courses and their respective marks registered for a student
    /// in a table view.
    /// </summary>
    /// <param name="student"></param>
    /// <returns>
    /// The selected <see cref="Course"/> from the displayed table view.
    /// <para>
    /// Returns null if no course is selected, or the student
    /// has not course registered for them
    /// </para>
    /// </returns>
    public Course? DisplayStudentScoreSheet(Student student)
    {
        ConsoleUIApp.ClearScreen();

        var courseMarks = FetchStudentMarks(student);
        if (courseMarks.Count == 0)
        {
            ConsoleHelper.ShowMessage("\nThis student has no marks recorded for any course... ");
            return null;
        }

        List<string> columnNames = [
            "Code", "Title", "Credits", "Attendance", "Assignment", "Exams",
            "Total Score", "Grade", "Grade Point"];

        var gpa = CalculateStudentGPA(courseMarks);
        var title = $"Student Score Sheet: {student}";
        var tableOptions = new TableBuilderOptions()
        {
            Columns = columnNames,
            Title = title,
        };
        var table = new TableView<CourseMarkViewModel>(courseMarks, tableOptions);

        table.Align(2, ColumnAlign.Right)
            .Align(3, ColumnAlign.Right)
            .Align(4, ColumnAlign.Right)
            .Align(5, ColumnAlign.Right)
            .Align(6, ColumnAlign.Right)
            .Align(8, ColumnAlign.Right)
            .AddFooter($"GPA = {gpa.ToString("0.00")}");

        var opts = table.Render();
        if (opts == -1) return null;

        var selectedCourseMark = courseMarks[opts];
        return dContext.Courses.Find(c => c.Code == selectedCourseMark.CourseCode);
    }

    private static float CalculateStudentGPA(List<CourseMarkViewModel> marks)
    {
        var totalCredits = 0;
        var totalGradePoints = 0f;

        foreach (var mark in marks)
        {
            totalCredits += mark.CreditHours;
            totalGradePoints += mark.GradePoint;
        }
        return totalGradePoints / totalCredits;
    }

    /// <summary>
    /// Fetch all the registered courses for a student from the database
    /// </summary>
    /// <returns>
    /// The list of courses, including their marks, registered by the provided
    /// student
    /// </returns>
    private List<CourseMarkViewModel> FetchStudentMarks(Student student)
    {
        var query = from course in dContext.Courses
                    join mark in dContext.StudentMarks
                    on course.Code equals mark.CourseCode
                    where mark.StudentId == student.Id
                    select new CourseMarkViewModel
                    {
                        CourseTitle = course.Title,
                        CourseCode = course.Code,
                        CreditHours = course.CreditHours,
                        Attendance = mark.Attendance,
                        Assignment = mark.Assignment,
                        EndOfTerm = mark.EndOfTerm,
                    };
        return query.ToList();
    }

    /// <summary>
    /// Displays the marks of a list of student who have registered
    /// for a selected course
    /// </summary>
    public void DisplayCourseSheet()
    {
        ConsoleHelper.PrintHeading("Select course to display sheet");
        var course = courses.GetCourse("Select Course to View Scoresheet");

        if (course == null)
        {
            ConsoleHelper.ShowMessage("\nOperation Aborted. No course selected... ");
            return;
        }

        List<string> columnNames = [
            "Id", "Student Name", "Attendance(5%)", "Assignment(25%)", "Exam(70%)",
            "Total(100%)", "Grade", "Grade Point"];

        var marks = FetchCourseStudentMarks(course);
        var tableOptions = new TableBuilderOptions()
        {
            Columns = columnNames,
            Title = $"Course Sheet: {course}"
        };
        var table = new TableView<StudentMarkViewModel>(marks, tableOptions);

        table.Align(2, ColumnAlign.Right)
            .Align(3, ColumnAlign.Right)
            .Align(4, ColumnAlign.Right)
            .Align(5, ColumnAlign.Right)
            .Align(6, ColumnAlign.Right)
            .Align(7, ColumnAlign.Right);

        table.Render();
    }

    /// <summary>
    /// Fetch a list of student (and their respective marks) who registered
    /// for this particular course
    /// </summary>
    /// <param name="course"></param>
    /// <returns></returns>
    private List<StudentMarkViewModel> FetchCourseStudentMarks(Course course)
    {
        var query = from student in dContext.Students
                    join mark in dContext.StudentMarks on student.Id equals mark.StudentId
                    where mark.CourseCode == course.Code
                    select new StudentMarkViewModel
                    {
                        StudentId = student.Id,
                        StudentName = $"{student.FirstName} {student.Surname}",
                        Attendance = mark.Attendance,
                        Assignment = mark.Assignment,
                        EndOfTerm = mark.EndOfTerm,
                    };
        return query.ToList();
    }
}
