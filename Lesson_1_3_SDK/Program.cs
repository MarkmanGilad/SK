using Lesson_1_2_http;


// OpenAI Call
//var completion = await OpenAI_SDK.Call();

// Gemini Call
var completion = await Gemini_SDK.Call();


Console.WriteLine($"OpenAI >> {completion}");