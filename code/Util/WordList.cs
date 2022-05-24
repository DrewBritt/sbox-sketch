namespace Sketch;

/// <summary>
/// Encapsulates a Sketch word list and it's associated metadata (name, creator, votes, etc.)
/// </summary>
public class WordList
{
    public string Name { get; set; }
    public string CreatorName { get; set; }
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
    public string[] Words { get; set; }
}
