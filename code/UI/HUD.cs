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
		public Panel StateInfo { get; set; }
		public Panel GamePanel { get; set; }
		public Panel Scoreboard { get; set; }
		public Panel DrawPanel { get; set; }
		public Panel DrawCanvas { get; set; }
		public Panel ToolsContainer { get; set; }
		public Panel ChatBox { get; set; }
		public Panel SelectWord { get; set; }
		public Panel CurrentDrawer { get; set; }

		public HUD()
		{
			if ( !IsClient )
				return;

			RootPanel.StyleSheet.Load( "/ui/HUD.scss" );
			StateInfo = RootPanel.AddChild<StateInfo>();

			GamePanel = RootPanel.Add.Panel("gamepanel");
			Scoreboard = GamePanel.AddChild<Scoreboard>();

			DrawPanel = GamePanel.Add.Panel("drawpanel");
			DrawCanvas = DrawPanel.AddChild<DrawCanvas>();
			ToolsContainer = DrawPanel.AddChild<ToolsContainer>();

			ChatBox = GamePanel.AddChild<ChatBox>();

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

		/// <summary>
		/// Recreates a fresh canvas texture on the called client.
		/// </summary>
		[ClientRpc]
		public void ClearCanvas()
        {
			var canvas = DrawCanvas as DrawCanvas;
			canvas.InitializeCanvasTexture();
        }

		/// <summary>
		/// Sends a list of pixels to update on the canvas.
		/// Much more performant than sending all image data every call.
		/// </summary>
		/// <param name="newPixels">Chunks of 4 ints: Index of pixel in array, followed by R, G, B values.</param>
		[ClientRpc]
		public void NetworkCanvasUpdate( int[] newPixels)
		{
			var p = DrawCanvas as DrawCanvas;
			for(int i = 0; i < newPixels.Length; i+=4)
			for(int i = 0; i < newPixels.Length-3; i+=4)
				p.UpdateCanvasInfo(newPixels[i], newPixels[i+1], newPixels[i+2], newPixels[i+3]);
			
		}
	}
}
