using System;
using System.Collections.Generic;
using Sandbox;

namespace Sketch;

/// <summary>
/// Handles random words for the players to draw.
/// </summary>
public static class Words
{
    public static List<WordList> PlayableLists = new List<WordList>();
    public const string ListPath = "/data/wordlist.json";

    /// <summary>
    /// Initializes Words.PlayableLists (duh dumbass) from base wordlist file.
    /// Errors out if file does not exist (installed with game so ideally should never throw?)
    /// </summary>
    public static void InitWordList()
    {
        if(FileSystem.Mounted.FileExists(ListPath))
        {
            //WordList = FileSystem.Mounted.ReadAllText(ListPath)
            var words = FileSystem.Mounted.ReadJson<string[]>(ListPath);
            var newList = new WordList()
            {
                Words = words
            };
            PlayableLists.Add(newList);
            return;
        }

        Log.Error("Sketch: Base WordList not found.");
    }

    /// <summary>
    /// Returns all words from all locally loaded word lists.
    /// </summary>
    /// <returns></returns>
    public static List<string> GetAllWords()
    {
        List<string> allWords = new List<string>();
        foreach(var list in PlayableLists)
            foreach(var word in list.Words)
                allWords.Add(word);

        return allWords;
    }

    /// <summary>
    /// Returns array containing "count" random words from all word lists. 
    /// </summary>
    /// <param name="count">Number of words to return.</param>
    /// <returns></returns>
    public static string[] RandomWords(int count)
    {
        string[] words = new string[count];
        var random = new Random();

        var allWords = GetAllWords();
        for(int i = 0; i < count; i++)
        {
            int randomIndex = random.Next(allWords.Count);
            words[i] = allWords[randomIndex];
        }

        return words;
    }
}
