using System.Text.Json;

public class Student
{
    public string Name { get; set; }
    public int Grade { get; set; }
    public string Subject { get; set; }
}

public class Country
{
    public string Name { get; set; }
    public double Population { get; set; }
}

public class CountriesResponse
{
    public List<Country> Countries { get; set; }
}

public static class JSON_example
{
    public static Task Run()
    {
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var student1 = new Student
        {
            Name = "Daniel",
            Grade = 95,
            Subject = "Math"
        };
        var student2 = new Student
        {
            Name = "Maya",
            Grade = 88,
            Subject = "CS"
        };
        string json = JsonSerializer.Serialize(student1);
        Console.WriteLine(json);

        var students = new List<Student> { student1, student2 };
        string jsonList = JsonSerializer.Serialize(students);
        Console.WriteLine(jsonList);

        // Single object
        string json1 = System.IO.File.ReadAllText("C:\\Users\\ASUS\\source\\repos\\SK\\Lesson_7_JSON\\student.json");
        Student student = JsonSerializer.Deserialize<Student>(json, jsonOptions);
        Console.WriteLine(student);
        // List of objects
        string jsonList1 = System.IO.File.ReadAllText("C:\\Users\\ASUS\\source\\repos\\SK\\Lesson_7_JSON\\student.json");
        List<Student> students1 = JsonSerializer.Deserialize<List<Student>>(jsonList, jsonOptions);
        Console.WriteLine(students1);

        return Task.CompletedTask;
    }
}
