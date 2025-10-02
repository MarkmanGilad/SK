using Lesson_10_Summarize;
using System.Runtime.InteropServices;
using System.Text;
using UglyToad.PdfPig.Graphics;

//var summarizer = new Summarizer();
var summarizer = new RefineSummarizer("English");

//string path = @"C:\Users\Gilad\source\repos\SK\Lesson_10_Summarize\PDF\AI-2024-HEB.pdf";
string path = @"C:\Users\Gilad\source\repos\SK\Lesson_10_Summarize\PDF\Thedangersofartificialintelligence.pdf";

string summary = await summarizer.Summarize(path);

string output = @"C:\Users\Gilad\source\repos\SK\Lesson_10_Summarize\PDF\summary4.txt";
File.WriteAllText(output, summary, Encoding.UTF8);

Console.WriteLine(summary); 