using Sandbox.UI;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sketch
{
	public partial class HUD : HudEntity<RootPanel>
	{
		public Panel GamePanel { get; set; }

		public HUD()
		{
			if ( !IsClient )
				return;

			RootPanel.StyleSheet.Load( "/ui/HUD.scss" );

			RootPanel.AddChild<Scoreboard>();

			GamePanel = RootPanel.Add.Panel( "gamescreen" );
			GamePanel.AddChild<StateInfo>();
			GamePanel.AddChild<DrawCanvas>();

			RootPanel.AddChild<ChatBox>();
		}
	}
}
