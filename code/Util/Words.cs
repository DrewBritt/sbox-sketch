using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Sketch
{
    /// <summary>
    /// Handles random words for the players to draw.
    /// </summary>
    public static class Words
    {
        private static List<string> WordList = new List<string>();
        public static string BaseListPath = "/data/wordlist.json";

        public static void AddToWordList(string path)
        {
            if(FileSystem.Mounted.FileExists(path))
            {
                var words = FileSystem.Mounted.ReadJson<string[]>(path).ToList();
                WordList.AddRange(words);
                return;
            }

            Log.Error($"Sketch: file at {path} not found.");
        }

        public static void RemoveFromWordList(string path)
        {
            if(FileSystem.Mounted.FileExists(path))
            {
                var words = FileSystem.Mounted.ReadJson<string[]>(path).ToList();
                foreach(var word in words)
                    WordList.Remove(word);
                return;
            }

            Log.Error($"Sketch: file at {path} not found.");
        }

        /// <summary>
        /// Returns array containing "count" random words from WordList. 
        /// </summary>
        /// <param name="count">Number of words to return.</param>
        /// <returns></returns>
        public static string[] RandomWords(int count)
        {
            string[] words = new string[count];
            var random = new Random();

            for(int i = 0; i < count; i++)
            {
                int randomIndex = random.Next(WordList.Count);
                words[i] = WordList[randomIndex];
            }

            return words;
        }
    }
}
