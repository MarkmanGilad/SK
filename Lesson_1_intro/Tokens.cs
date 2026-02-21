using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML.Tokenizers; // install using NuGet package: Microsoft.ML.Tokenizers

//https://platform.openai.com/tokenizer

namespace Lesson_1_intro
{
    public class Tokens
    {
        private readonly Tokenizer _tokenizer = TiktokenTokenizer.CreateForModel("gpt-4o");

        public void RunDemo()
        {
            var tokens = EncodeToTokens("The wizard cast a spell called Glimmer-shimmer-puff.");
            PrintTokens(tokens);
            tokens = EncodeToTokens("internationalization is important.");
            PrintTokens(tokens);
            tokens = EncodeToTokens("The microbiologist studied the extremophile's DNA");
            PrintTokens(tokens);
            Console.WriteLine("*********************************************************");
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    int t = 199991 + i;
                    var token = GetEncodedTokenById(t);
                    Console.WriteLine($"{t}: {token?.Value}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"end {e.Message}");
                }

            }

        }

        public EncodedToken? GetEncodedTokenById(int id)
        {
            var tokenText = _tokenizer.Decode([id]);
            var tokens = _tokenizer.EncodeToTokens(tokenText, out _);
            if (tokens.Count == 0)
                return null;

            return tokens[0];
        }

        public List<EncodedToken> EncodeToTokens(string text)
        {
            return _tokenizer.EncodeToTokens(text, out _).ToList();
        }

        public void PrintTokens(List<EncodedToken> tokens)
        {
            foreach (var token in tokens)
            {
                Console.WriteLine($"{token.Id}\t\t{token.Value}");
            }
        }

        public List<int> Encode(string text)
        {
            return _tokenizer.EncodeToIds(text).ToList();
        }

        public string IdToTokenText(int id)
        {
            return _tokenizer.Decode([id]);
        }
    }
}


