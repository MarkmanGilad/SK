using Lesson_11._5_Embeddings;

var embed = new EmbeddingDemo();

string sentenceA = "The queen sat on her throne.";
string sentenceB = "The king ruled the kingdom.";
string sentenceC = "I ate a delicious apple pie.";

// 3. Generate embeddings
var embA = await embed.EmbedAsync(sentenceA);
var embB = await embed.EmbedAsync(sentenceB);
var embC = await embed.EmbedAsync(sentenceC);

// 4. Compute similarities
double simAB = embed.CosineSimilarity(embA, embB);
double simAC = embed.CosineSimilarity(embA, embC);
double simBC = embed.CosineSimilarity(embB, embC);

Console.WriteLine($"Similarity Queen vs King:   {simAB:F3}");
Console.WriteLine($"Similarity Queen vs Apple:  {simAC:F3}");
Console.WriteLine($"Similarity King  vs Apple:  {simBC:F3}");




Console.WriteLine("****************************************************");

string s1 = """
    Yesterday evening, Maria went to a local Italian restaurant with her friends. 
    They ordered pasta, pizza, and salad, and spent almost two hours talking about work and family. 
    The atmosphere was lively, with music playing in the background, and they all enjoyed a pleasant meal together before heading home.
    """;
string s2 = """
    Last night, Maria met some friends at a nearby dining place. 
    They had a long dinner filled with conversation about their jobs and relatives, while sharing dishes like spaghetti, pizza, and fresh vegetables. 
    The setting was cheerful, with music in the air, and everyone left satisfied after the enjoyable evening.
    """;
string s3 = "She ate dinner at a restaurant last night.";
string s4 = "Yesterday evening, she had her meal in a café.";
string s5 = "Maria";
string s6 = "dog";

//Generate embeddings
var emb1 = await embed.EmbedAsync(s1);
var emb2 = await embed.EmbedAsync(s2);
var emb3 = await embed.EmbedAsync(s3);
var emb4 = await embed.EmbedAsync(s4);
var emb5 = await embed.EmbedAsync(s5);
var emb6 = await embed.EmbedAsync(s6);

//Compute similarities
double sim12 = embed.CosineSimilarity(emb1, emb2);
double sim34 = embed.CosineSimilarity(emb3, emb4);
double sim15 = embed.CosineSimilarity(emb1, emb5);
double sim25 = embed.CosineSimilarity(emb2, emb5);
double sim35 = embed.CosineSimilarity(emb3, emb5);
double sim16 = embed.CosineSimilarity(emb1, emb6);

Console.WriteLine($"Similarity long dinner paragraph:   {sim12:F3}");
Console.WriteLine($"Similarity short dinner:  {sim34:F3}");
Console.WriteLine($"Similarity dinner1 vs maria:  {sim15:F3}");
Console.WriteLine($"Similarity dinner2 vs maria:  {sim25:F3}");
Console.WriteLine($"Similarity dinner3 vs maria:  {sim35:F3}");
Console.WriteLine($"Similarity dinner1 vs dog:  {sim16:F3}");