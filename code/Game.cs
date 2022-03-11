using Sandbox;
using Sandbox.UI;
using System;
using System.Linq;

namespace Sketch
{
	public partial class Game :	Sandbox.Game
	{
		public static new Game Current => Sandbox.Game.Current as Game;

		public HUD Hud { get; set; }

		public Game()
		{	
			if (IsServer)
			{
				Words.InitWordList();
				Hud = new HUD();
			}
		}

		public override void ClientJoined( Client cl )
		{
			ChatBox.AddInformation( To.Everyone, $"{cl.Name} has joined the game!");
		}

		public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
		{
			ChatBox.AddInformation( To.Everyone, $"{cl.Name} has left ({reason})" );
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			Hud?.Delete();
			Hud = null;
		}

		/// <summary>
		/// Called to set the camera up, clientside only.
		/// </summary>
		Angles angles;
		public override CameraSetup BuildCamera( CameraSetup camSetup )
		{
			angles += new Angles( 0, -1.0f, 0.0f ) * RealTime.Delta;

			camSetup.Rotation = Rotation.From( angles );
			camSetup.Position = new Vector3( 0, 0, 130);
			camSetup.FieldOfView = 80;
			camSetup.Ortho = false;
			camSetup.Viewer = null;

			return camSetup;
		}

		public override bool CanHearPlayerVoice( Client source, Client dest )
		{
			Host.AssertServer();

			return true;
		}
	}
}
