namespace ConsoleUI.App.ViewModels;

public class MarksViewModel
{
    public int Attendance { get; set; }
    public int Assignment { get; set; }
    public int EndOfTerm { get; set; }
    public int CreditHours { get; set; }

    public int TotalScore => Attendance + Assignment + EndOfTerm;

    public float GradePoint => Grade.Point * CreditHours;

    private (string Grade, float Point)? _grade; 
    public (string Grade, float Point) Grade
    {
        get
        {
            _grade ??= GetGrade();
            return _grade.Value;
        }
    }

    private (string Grade, float Point) GetGrade()
    {
        var score = TotalScore;
        if (score >= 80) return ("A", 4.0f);
        if (score >= 75) return ("B+", 3.5f);
        if (score >= 70) return ("B", 3.0f);
        if (score >= 65) return ("C+", 2.5f);
        if (score >= 60) return ("C", 2.0f);
        if (score >= 55) return ("D+", 1.5f);
        if (score >= 50) return ("D+", 1.0f);
        if (score >= 40) return ("E", 0.5f);
        return ("F", 0);
    }
}
