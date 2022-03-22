using Sandbox;
using Sandbox.UI;

namespace Sketch
{
    /// <summary>
    /// Contains input fields and buttons to let admins control the game.
    /// </summary>
    public partial class Settings : Panel
    {
        public Panel Container;

        public Settings()
        {
            StyleSheet.Load("/ui/Settings.scss");

            Container = Add.Panel("container");
        }

        [Event.BuildInput]
        public void BuildInput(InputBuilder b)
        {
            if(b.Pressed(InputButton.Menu))
            {
                SetClass("open", !HasClass("open"));
            }
                
        }
    }
}
