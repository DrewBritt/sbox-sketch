using System.Linq;
using Sandbox;
using Sketch.UI;

namespace Sketch
{
    public partial class Game : Sandbox.Game
    {
        public static new Game Current => Sandbox.Game.Current as Game;

        public Hud Hud { get; set; }

        public Game() : base()
        {
            if(IsServer)
            {
                Words.AddToWordList(Words.BaseListPath);
                PrecacheInit();
                Hud = new Hud();
            }
        }

        public override void ClientJoined(Client cl)
        {
            ChatBox.AddInformation(To.Everyone, (ulong)cl.PlayerId, $"{cl.Name} has joined the game!");

            // Set voice to 2D
            cl.VoiceStereo = false;
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
    }
}
