namespace ConsoleUI.App.Models;

public class StudentMark
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string CourseCode { get; set; } = null!;
    public int Attendance { get; set; } // 5%
    public int Assignment { get; set; } // 25 %
    public int EndOfTerm { get; set; } // 70%
}
