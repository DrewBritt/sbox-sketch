using Sandbox.UI;
using Sandbox;
using Sandbox.UI.Construct;

namespace Sketch
{
	public partial class CurrentDrawer : Panel
	{
		public Panel Container, TextBox;
		public Label DrawerText;
		private RealTimeSince panelOpened;

		public CurrentDrawer()
		{
			StyleSheet.Load( "/ui/CurrentDrawer.scss" );

			Container = Add.Panel( "container" );
			TextBox = Container.Add.Panel( "textbox" );
			DrawerText = TextBox.Add.Label( "_ is selecting a word.", "drawertext" );
		}

		public override void Tick()
		{
			var game = Game.Current;
			if ( game == null ) return;

			if ( panelOpened > 3 )
			{
				Container.RemoveClass( "open" );
				Container.AddClass( "closed" );
			}

		}

		public void DisplayCurrentDrawer()
		{
			DrawerText.Text = $"{Client.All[Game.Current.CurrentDrawerIndex].Name} is selecting a word.";

			Container.AddClass( "open" );
			Container.RemoveClass( "closed" );
			panelOpened = 0;
		}
	}
}
