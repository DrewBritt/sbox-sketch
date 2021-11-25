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
		public Panel Scoreboard { get; set; }
		public Panel GamePanel { get; set; }
		public Panel StateInfo { get; set; }
		public Panel DrawCanvas { get; set; }
		public Panel ChatBox { get; set; }
		public Panel SelectWord { get; set; }

		public HUD()
		{
			if ( !IsClient )
				return;

			RootPanel.StyleSheet.Load( "/ui/HUD.scss" );

			Scoreboard = RootPanel.AddChild<Scoreboard>();

			GamePanel = RootPanel.Add.Panel( "gamescreen" );
			StateInfo = GamePanel.AddChild<StateInfo>();
			DrawCanvas = GamePanel.AddChild<DrawCanvas>();

			ChatBox = RootPanel.AddChild<ChatBox>();

			SelectWord = RootPanel.AddChild<SelectWord>();
		}

		[ClientRpc]
		public void OpenCurrentDrawerPopup()
		{
			var panel = SelectWord as SelectWord;
			panel.OpenPanel();
		}
	}
}
