using Sandbox;
using Sandbox.UI;

namespace Sketch;

/// <summary>
/// Contains input fields and buttons to let admins control the game.
/// </summary>
[UseTemplate]
public partial class Settings : NavPanel
{
    public Settings()
    {
        Navigate("settings/gamesettings");
    }

    [Event.BuildInput]
    public void BuildInput(InputBuilder b)
    {
        if(b.Pressed(InputButton.Menu) && ConsoleSystem.GetValue("sv_cheats") == "1")
        {
            SetClass("open", !HasClass("open"));
        }
    }
}
