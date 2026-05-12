
//var imgGen = new OpenAI_Img();
//var imgGen = new GoogleImg();


//Console.Write("Enter image prompt >> ");
//string prompt = Console.ReadLine()!;

//await imgGen.GenerateImageAsync(prompt, "generated11.png");
//Console.WriteLine("Image created successfully!");

/********************************************/

var voiceGen = new OpenAI_Voice();
//var voiceGen = new Google_Voice();
Console.Write("Enter voice prompt >> ");
string voicePrompt = Console.ReadLine();
await voiceGen.GenerateVoiceAsync(voicePrompt, "generated12.mp3");   //mp3 - OpenAI
Console.WriteLine("Voice created successfully!");