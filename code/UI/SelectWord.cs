using Sandbox.UI;
using Sandbox;
using Sandbox.UI.Construct;

namespace Sketch
{
	public partial class SelectWord : Panel
	{
		public Panel Container;
		private RealTimeSince panelOpened;

		public SelectWord()
		{
			StyleSheet.Load( "/ui/SelectWord.scss" );

			Container = Add.Panel( "container" );
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

		public void DisplayWordPool()
		{

		}
	}
}
