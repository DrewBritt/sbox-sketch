using Sandbox;
using Sandbox.UI;
using System;
using System.Linq;

namespace Sketch
{
	[Library( "Sketch" ), Hammer.Skip]
	public partial class Game : GameBase
	{
		public static Game Current { get; protected set; }
		public HUD Hud { get; set; }

		public Game()
		{
			Transmit = TransmitType.Always;
			Current = this;
			
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

		public override void PostLevelLoaded() { }

		public override void Shutdown()
		{
			if (Current == this)
			{
				Current = null;
			}
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

		/// <summary>
		/// Called after the camera setup logic has run. Allow the gamemode to 
		/// do stuff to the camera, or using the camera. Such as positioning entities 
		/// relative to it, like viewmodels etc.
		/// </summary>
		public override void PostCameraSetup( ref CameraSetup camSetup )
		{
			if ( Local.Pawn != null )
			{
				// VR anchor default is at the pawn's location
				VR.Anchor = Local.Pawn.Transform;

				Local.Pawn.PostCameraSetup( ref camSetup );
			}

			// Position any viewmodels
			BaseViewModel.UpdateAllPostCamera( ref camSetup );
			CameraModifier.Apply( ref camSetup );
		}

		public override bool CanHearPlayerVoice( Client source, Client dest )
		{
			Host.AssertServer();

			return true;
		}

		public override void OnVoicePlayed( long playerId, float level )
		{
			VoiceList.Current?.OnVoicePlayed( playerId, level );
		}
	}
}
