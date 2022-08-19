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

        public Settings()
        {
            MaxRounds.Bind("text", () => Game.MaxRounds);
            DrawTime.Bind("text", () => Game.DrawTime);
            WordPoolSize.Bind("text", () => Game.WordPoolSize);
            SelectTime.Bind("text", () => Game.SelectWordTime);
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
