using Sandbox.UI;
using Sandbox;
using Sandbox.UI.Construct;

namespace Sketch
{
	public partial class StateInfo : Panel
	{
		public Panel Container;
		public Label StateName, StateTime;

		public Label BlankLetters;

		public StateInfo()
		{
			StyleSheet.Load( "/ui/StateInfo.scss" );

			Container = Add.Panel( "container" );
			StateName = Container.Add.Label( "State", "statename" );
			StateTime = Container.Add.Label( "00:00", "statetime" );
			BlankLetters = Container.Add.Label( "", "blankletters" );
		}

		public override void Tick()
		{
			var game = Game.Current;
			if ( game == null ) return;

			StateName.Text = game.CurrentStateName;
			StateTime.Text = game.CurrentStateTime;

			BlankLetters.Text = game.CurrentLettersString();
		}
	}
}
