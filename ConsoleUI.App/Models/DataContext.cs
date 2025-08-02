using System.Text.Json;

namespace ConsoleUI.App.Models;

public class DataContext
{
    public List<Student> Students { get; set; } = [];
    public List<Course> Courses { get; set; } = [];
    public List<StudentMark> StudentMarks { get; set; } = [];

    private string _filename = null!;

    public DataContext() { } // for json serialization

    public DataContext(string fn)
    {
        _filename = fn;
        LoadData();
    }

    public void SaveChanges()
    {
        var json = JsonSerializer.Serialize(this);
        File.WriteAllText(_filename, json);
    }

    private void LoadData()
    {
        var json = File.ReadAllText(_filename);

        if (json.Trim().Length == 0) return;

        var d = JsonSerializer.Deserialize<DataContext>(json)!;

        Students = d.Students;
        Courses = d.Courses;
        StudentMarks = d.StudentMarks;
    }
}
