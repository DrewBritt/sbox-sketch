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
        public TextEntry MaxRounds { get; set; }
        public TextEntry DrawTime { get; set; }
        public TextEntry WordPoolSize { get; set; }
        public TextEntry SelectTime { get; set; }
        public string MaxRoundsString => Game.MaxRounds.ToString();
        public string DrawTimeString => Game.DrawTime.ToString();
        public string WordPoolSizeString => Game.WordPoolSize.ToString();
        public string SelectTimeString => Game.SelectWordTime.ToString();

        public Settings()
        {
            MaxRounds.Bind("text", this, "MaxRoundsString");
            DrawTime.Bind("text", this, "DrawTimeString");
            WordPoolSize.Bind("text", this, "WordPoolSizeString");
            SelectTime.Bind("text", this, "SelectTimeString");
        }

        public void SetMaxRounds() => ConsoleSystem.Run($"sketch_maxrounds {MaxRounds.Text}");
        public void SetDrawTime() => ConsoleSystem.Run($"sketch_drawtime {DrawTime.Text}");
        public void SetWordPoolSize() => ConsoleSystem.Run($"sketch_wordpoolsize {WordPoolSize.Text}");
        public void SetSelectTime() => ConsoleSystem.Run($"sketch_selectwordtime {SelectTime.Text}");
        public void ResetGame() => ConsoleSystem.Run($"sketch_resetgame");
        public void SkipWord() => ConsoleSystem.Run($"sketch_skipword");

        [Event.BuildInput]
        public void BuildInput(InputBuilder b)
        {
            if(b.Pressed(InputButton.Menu) && ConsoleSystem.GetValue("sv_cheats") == "1")
            {
                SetClass("open", !HasClass("open"));
            }
        }
    }
}
