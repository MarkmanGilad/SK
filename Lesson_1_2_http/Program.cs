using Lesson_1_2_http;

// OpenAI Call
var completion = await OpenAI_Http.Call();

// Gemini Call
//var completion = await Gemini_Http.Call();


Console.WriteLine($"OpenAI >> {completion}");
