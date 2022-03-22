using Sandbox;
using Sandbox.UI;

namespace Sketch
{
    /// <summary>
    /// Contains input fields and buttons to let admins control the game.
    /// </summary>
    [UseTemplate]
    public partial class Settings : Panel
    {
        public int PlayTime { get; set; }
        public int MaxRounds { get; set; }
        public int SelectTime { get; set; }
        public int WordPoolSize { get; set; }

        public Settings()
        {

        }

        public void SetPlayTime() => ConsoleSystem.Run($"sketch_playtime {PlayTime}");
        public void SetMaxRounds() => ConsoleSystem.Run($"sketch_maxrounds {MaxRounds}");
        public void SetSelectTime() => ConsoleSystem.Run($"sketch_selectwordtime {SelectTime}");
        public void SetWordPoolSize() => ConsoleSystem.Run($"sketch_wordpoolsize {WordPoolSize}");
        public void ResetGame() => ConsoleSystem.Run($"sketch_resetgame");
        public void SkipWord() => ConsoleSystem.Run($"sketch_skipword");

        [Event.BuildInput]
        public void BuildInput(InputBuilder b)
        {
            if(b.Pressed(InputButton.Menu) && ConsoleSystem.GetValue("sv_cheats") == "1")
            {
                SetClass("open", !HasClass("open"));
                PlayTime = Game.Current.PlayTime;
                MaxRounds = Game.Current.MaxRounds;
                SelectTime = Game.Current.SelectWordTime;
                WordPoolSize = Game.Current.WordPoolSize;
            }
        }
    }
}
