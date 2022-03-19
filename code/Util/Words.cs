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
        public static List<string> WordList;
        public static string ListPath = "Util/wordlist.txt";

        /// <summary>
        /// Initializes Words.WordList (duh dumbass) from local file.
        /// Errors out if file does not exist.
        /// </summary>
        public static void InitWordList()
        {
            if (FileSystem.Mounted.FileExists(ListPath))
            {
                WordList = FileSystem.Mounted.ReadAllText(ListPath).Split('\n', StringSplitOptions.TrimEntries).ToList();
                return;
            }

            Log.Error("Sketch: WordList not loaded.");
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

            for (int i = 0; i < count; i++)
            {
                int randomIndex = random.Next(WordList.Count);
                words[i] = WordList[randomIndex];
            }

            return words;
        }
    }
}
