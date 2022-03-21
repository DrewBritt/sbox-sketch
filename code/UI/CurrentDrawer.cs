using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Sketch
{
    public partial class CurrentDrawer : Panel
    {
        public Panel Container;
        public Image Avatar;
        public Label DrawerText;
        private RealTimeSince panelOpened;

        public CurrentDrawer()
        {
            StyleSheet.Load("/ui/CurrentDrawer.scss");

            Container = Add.Panel("container");
            Avatar = Container.Add.Image();
            DrawerText = Container.Add.Label("_ is selecting a word.", "drawertext");
        }

        public override void Tick()
        {
            var game = Game.Current;
            if (game == null) return;

            if (panelOpened > 3)
            {
                RemoveClass("open");
                AddClass("closed");
            }

        }

        public void DisplayCurrentDrawer()
        {
            var drawer = Game.Current.CurrentDrawer;
            Avatar.SetTexture($"avatar:{drawer.PlayerId}");
            DrawerText.Text = $"{drawer.Name} is selecting a word.";

            AddClass("open");
            RemoveClass("closed");
            panelOpened = 0;
        }
    }
}
