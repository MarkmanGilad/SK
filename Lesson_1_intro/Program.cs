using Lesson_1_intro;

// OpenAI Call
//var completion = await OpenAI_Http.Call();
//var completion = await OpenAI_SDK.Call();
//var completion = await OpenAI_SK.Call();


// Gemini Call
//var completion = await Gemini_Http.Call();
//var completion = await Gemini_SDK.Call();
//var completion = await Gemini_SK.Call();


//Console.WriteLine($"OpenAI >> {completion}");

var tokenizer = new Tokens();
tokenizer.RunDemo();
