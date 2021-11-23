using Sandbox;
using Sandbox.UI;
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

			if(IsServer)
			{
				Words.InitWordList();
				Hud = new HUD();
			}
		}
		public override void ClientJoined( Client cl )
		{
			ChatBox.AddInformation( To.Everyone, $"{cl.Name} has joined the game!", $"avatar:{cl.PlayerId}" );


		}

		public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
		{
			ChatBox.AddInformation( To.Everyone, $"{cl.Name} has left ({reason})", $"avatar:{cl.PlayerId}" );

			if(cl.Pawn.IsValid())
			{
				cl.Pawn.Delete();
				cl.Pawn = null;
			}
		}

		/// <summary>
		/// Called each tick.
		/// Serverside: Called for each client every tick
		/// Clientside: Called for each tick for local client. Can be called multiple times per tick.
		/// </summary>
		public override void Simulate( Client cl )
		{
			if ( !cl.Pawn.IsValid() ) return;

			// Block Simulate from running clientside
			// if we're not predictable.
			if ( !cl.Pawn.IsAuthority ) return;

			cl.Pawn.Simulate( cl );
		}

		/// <summary>
		/// Called each frame on the client only to simulate things that need to be updated every frame. An example
		/// of this would be updating their local pawn's look rotation so it updates smoothly instead of at tick rate.
		/// </summary>
		public override void FrameSimulate( Client cl )
		{
			Host.AssertClient();

			if ( !cl.Pawn.IsValid() ) return;

			// Block Simulate from running clientside
			// if we're not predictable.
			if ( !cl.Pawn.IsAuthority ) return;

			cl.Pawn?.FrameSimulate( cl );
		}

		public override void PostLevelLoaded()
		{
		}

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

			/*
			Hud?.Delete();
			Hud = null;
			*/
		}

		[Predicted]
		public Camera LastCamera { get; set; }

		/// <summary>
		/// Which camera should we be rendering from?
		/// </summary>
		public virtual ICamera FindActiveCamera()
		{
			if ( Local.Client.DevCamera != null ) return Local.Client.DevCamera;
			if ( Local.Client.Camera != null ) return Local.Client.Camera;
			if ( Local.Pawn != null ) return Local.Pawn.Camera;

			return null;
		}

		/// <summary>
		/// Called to set the camera up, clientside only.
		/// </summary>
		public override CameraSetup BuildCamera( CameraSetup camSetup )
		{
			var cam = FindActiveCamera();

			if ( LastCamera != cam )
			{
				LastCamera?.Deactivated();
				LastCamera = cam as Camera;
				LastCamera?.Activated();
			}

			cam?.Build( ref camSetup );

			PostCameraSetup( ref camSetup );

			return camSetup;
		}

		/// <summary>
		/// Clientside only. Called every frame to process the input.
		/// The results of this input are encoded\ into a user command and
		/// passed to the PlayerController both clientside and serverside.
		/// This routine is mainly responsible for taking input from mouse/controller
		/// and building look angles and move direction.
		/// </summary>
		public override void BuildInput( InputBuilder input )
		{
			Event.Run( "buildinput", input );

			// the camera is the primary method here
			LastCamera?.BuildInput( input );

			Local.Pawn?.BuildInput( input );
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
