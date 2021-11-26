﻿using Sandbox.UI;
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

		[ClientRpc]
		public void DisplayCurrentDrawer()
		{
			var panel = CurrentDrawer as CurrentDrawer;
			panel.DisplayCurrentDrawer();
		}

		[ClientRpc]
		public void SendWordPool( string[] pool, int time )
		{
			var p = SelectWord as SelectWord;
			p.Pool = pool;
			p.DisplayWordPool(time);
		}
	}
}
