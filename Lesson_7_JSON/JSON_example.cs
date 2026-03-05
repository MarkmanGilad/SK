using System.Text.Json;

public static class JSON_example
{
    public static async Task Run()
    {
        var student1 = new Student
        {
            Name = "Daniel",
            Grade = 95,
            Subject = "Math"
        };
        string json = JsonSerializer.Serialize(student1);
        Console.WriteLine(json);

        var student2 = new Student
        {
            Name = "Maya",
            Grade = 88,
            Subject = "CS"
        };
        
        var students = new List<Student> { student1, student2 };
        string jsonList = JsonSerializer.Serialize(students);
        Console.WriteLine(jsonList);
                
        // Single object
        string json1 = System.IO.File.ReadAllText("C:\\Users\\ASUS\\source\\repos\\SK\\Lesson_7_JSON\\student.json");
        Student student = JsonSerializer.Deserialize<Student>(json);
        Console.WriteLine(student);

        // List of objects
        string jsonList1 = System.IO.File.ReadAllText("C:\\Users\\ASUS\\source\\repos\\SK\\Lesson_7_JSON\\student.json");
        
        var jsonOptions = new JsonSerializerOptions {PropertyNameCaseInsensitive = true };
        List<Student> students1 = JsonSerializer.Deserialize<List<Student>>(
            jsonList, jsonOptions);
        
        Console.WriteLine(students1);

    }
}
