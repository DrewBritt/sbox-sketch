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
		public Panel CurrentDrawer { get; set; }

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
			CurrentDrawer = RootPanel.AddChild<CurrentDrawer>();
		}

		/// <summary>
		/// Popup to display current drawer pre-round.
		/// </summary>
		[ClientRpc]
		public void DisplayCurrentDrawer()
		{
			var panel = CurrentDrawer as CurrentDrawer;
			panel.DisplayCurrentDrawer();
		}

		/// <summary>
		/// Sends Word Pool to drawer to display elements on screen
		/// </summary>
		/// <param name="pool"></param>
		/// <param name="time"></param>
		[ClientRpc]
		public void SendWordPool( string[] pool, int time )
		{
			var p = SelectWord as SelectWord;
			p.Pool = pool;
			p.DisplayWordPool(time);
		}

		/// <summary>
		/// Sends current letters to client. 
		/// If guessers receive this, it's the currentletters with underscores.
		/// If drawer receives this, it's once at the beginning of the round with the word.
		/// </summary>
		/// <param name="currentletters"></param>
		[ClientRpc]
		public void SendCurrentLetters(string currentletters)
		{
			var p = StateInfo as StateInfo;
			p.BlankLetters.Text = currentletters;
		}
	}
}
