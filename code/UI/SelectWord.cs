using Sandbox.UI;
using Sandbox;
using Sandbox.UI.Construct;

namespace Sketch
{
	public partial class SelectWord : Panel
	{
		public Panel Container, TextBox;
		public Label DrawerText;
		private RealTimeSince panelOpened;

		public SelectWord()
		{
			StyleSheet.Load( "/ui/SelectWord.scss" );

			Container = Add.Panel( "container" );
			TextBox = Container.Add.Panel( "textbox" );
			DrawerText = TextBox.Add.Label( "_ is selecting a word.", "drawertext" );
		}

		public override void Tick()
		{
			var game = Game.Current;
			if ( game == null ) return;

			if(panelOpened > 3)
			{
				Container.RemoveClass( "open" );
				Container.AddClass( "closed" );
			}

		}

		public void OpenPanel()
		{
			DrawerText.Text = $"{Client.All[Game.Current.CurrentDrawerIndex].Name} is selecting a word.";

			Container.AddClass( "open" );
			Container.RemoveClass( "closed" );
			panelOpened = 0;
		}
	}
}
