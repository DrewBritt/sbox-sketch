using System;
using System.Collections.Generic;
using Sandbox;

namespace Sketch;

/// <summary>
/// Handles random words for the players to draw.
/// </summary>
public static class Words
{
    private static List<WordList> LoadedLists = new List<WordList>();
    private const string BaseListPath = "/data/wordlists/basewordlist.json";

    /// <summary>
    /// Loads a Sketch Wordlist JSON file.
    /// </summary>
    /// <param name="path">Path of .JSON Sketch Wordlist.</param>
    public static void LoadWordList(string path)
    {
        if(FileSystem.Mounted.FileExists(BaseListPath))
        {
            WordList list = FileSystem.Mounted.ReadJson<WordList>(path);
            LoadedLists.Add(list);
            return;
        }

        Log.Error("Sketch: Base WordList not found.");
    }

    public static void RemoveWordList(WordList list) => LoadedLists.Remove(list);

    /// <summary>
    /// Loads Base Wordlist into LoadedLists.
    /// </summary>
    public static void InitBaseList()
    {
        LoadWordList(BaseListPath);
    }

    /// <summary>
    /// Returns all words from all locally loaded word lists.
    /// </summary>
    public static List<string> GetAllWords()
    {
        List<string> allWords = new List<string>();
        foreach(var list in LoadedLists)
            foreach(var word in list.Words)
                allWords.Add(word);

        return allWords;
    }

    /// <summary>
    /// Returns array containing "count" random words from all word lists. 
    /// </summary>
    /// <param name="count">Number of words to return.</param>
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
