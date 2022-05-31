using Sandbox;
using Sandbox.UI;

namespace Sketch;

[UseTemplate]
[NavTarget("settings/gamesettings")]
public partial class GameSettings : Panel
{
    public int PlayTime { get; set; }
    public int MaxRounds { get; set; }
    public int SelectTime { get; set; }
    public int WordPoolSize { get; set; }

    public GameSettings()
    {
        PlayTime = Game.Current.PlayTime;
        MaxRounds = Game.Current.MaxRounds;
        SelectTime = Game.Current.SelectWordTime;
        WordPoolSize = Game.Current.WordPoolSize;
    }

    public void SetPlayTime() => ConsoleSystem.Run($"sketch_playtime {PlayTime}");
    public void SetMaxRounds() => ConsoleSystem.Run($"sketch_maxrounds {MaxRounds}");
    public void SetSelectTime() => ConsoleSystem.Run($"sketch_selectwordtime {SelectTime}");
    public void SetWordPoolSize() => ConsoleSystem.Run($"sketch_wordpoolsize {WordPoolSize}");
    public void ResetGame() => ConsoleSystem.Run($"sketch_resetgame");
    public void SkipWord() => ConsoleSystem.Run($"sketch_skipword");
}
