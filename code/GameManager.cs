using System.Linq;
using System.Security.Cryptography;
using Sandbox;
using Sketch.UI;

namespace Sketch
{
    public partial class GameManager : Sandbox.GameManager
    {
        public static new GameManager Current => Sandbox.GameManager.Current as GameManager;

        public Hud Hud { get; set; }

        public GameManager() : base()
        {
            if(Game.IsServer)
            {
                Words.AddToWordList(Words.BaseListPath);
                PrecacheInit();
                Hud = new Hud();
            }
        }

        public override void ClientJoined(IClient cl)
        {
            ChatBox.AddInformation(To.Everyone, (ulong)cl.SteamId, $"{cl.Name} has joined the game!");

            // Set voice to 2D
            cl.Voice.WantsStereo = false;
        }

        public override void ClientDisconnect(IClient cl, NetworkDisconnectionReason reason)
        {
            ChatBox.AddInformation(To.Everyone, (ulong)cl.SteamId, $"{cl.Name} has left ({reason})");
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
        /*Angles angles;
        public override CameraSetup BuildCamera(CameraSetup camSetup)
        {
            angles += new Angles(0, -1.0f, 0.0f) * RealTime.Delta;

            camSetup.Rotation = Rotation.From(angles);
            camSetup.Position = new Vector3(0, 0, 130);
            camSetup.FieldOfView = 80;
            camSetup.Ortho = false;
            camSetup.Viewer = null;

            return camSetup;
        }*/

        public override bool CanHearPlayerVoice(IClient source, IClient dest)
        {
            Game.AssertServer();

            return true;
        }
    }
}
