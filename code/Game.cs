using System.Linq;
using Sandbox;

namespace Sketch
{
    public partial class Game : Sandbox.Game
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

        public override void ClientJoined(Client cl)
        {
            ChatBox.AddInformation(To.Everyone, (ulong)cl.PlayerId, $"{cl.Name} has joined the game!");
        }

        public override void ClientDisconnect(Client cl, NetworkDisconnectionReason reason)
        {
            ChatBox.AddInformation(To.Everyone, (ulong)cl.PlayerId, $"{cl.Name} has left ({reason})");
            Sound.FromScreen("doorshutting");
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
        public override CameraSetup BuildCamera(CameraSetup camSetup)
        {
            angles += new Angles(0, -1.0f, 0.0f) * RealTime.Delta;

            camSetup.Rotation = Rotation.From(angles);
            camSetup.Position = new Vector3(0, 0, 130);
            camSetup.FieldOfView = 80;
            camSetup.Ortho = false;
            camSetup.Viewer = null;

            return camSetup;
        }

        public override bool CanHearPlayerVoice(Client source, Client dest)
        {
            Host.AssertServer();

            return true;
        }

        /// <summary>
		/// Someone is speaking via voice chat. This might be someone in your game,
		/// or in your party, or in your lobby.
		/// </summary>
		public override void OnVoicePlayed(long playerId, float level)
        {
            var client = Client.All.Where(x => x.PlayerId == playerId).FirstOrDefault();
            if (client.IsValid())
            {
                client.VoiceLevel = level;
                client.TimeSinceLastVoice = 0;
            }

            Log.Info(playerId);
            (Current.Hud.Scoreboard as Scoreboard).OnVoicePlayed(playerId, level);
        }
    }
}
