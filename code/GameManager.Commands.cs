using System;
using Sandbox;

namespace Sketch
{
    public partial class GameManager
    {
        /// <summary>
        /// Generic handler for invalid command arguments.
        /// </summary>
        /// <param name="errormessage"></param>
        [ClientRpc]
        public void CommandError(string errormessage)
        {
            Log.Error(errormessage);
        }

        /// <summary>
        /// Sends new color's RGBA data, deserializes, and sets on Game.Current
        /// </summary>
        /// <param name="rgba"></param>
        [ConCmd.Server]
        public static void SetCurrentColor(string rgba)
        {
            if(ConsoleSystem.Caller != Current.CurrentDrawer)
                return;

            //Split rgba string "rgba( r, g, b, a )" into [r, g, b, a]
            //Substring cuts off "rgba( " and IndexOf gets the final parenthesis
            string[] bytes = rgba.Substring(6, rgba.IndexOf(')') - 6).Split(',', StringSplitOptions.TrimEntries);

            Color32 newColor = new Color32((byte)bytes[0].ToInt(), (byte)bytes[1].ToInt(), (byte)bytes[2].ToInt());
            Current.CurrentColor = newColor;
        }

        /// <summary>
        /// Sends new brush size and sets on Game.Current
        /// </summary>
        /// <param name="size"></param>
        [ConCmd.Server]
        public static void SetCurrentSize(int size)
        {
            if(ConsoleSystem.Caller != Current.CurrentDrawer)
                return;

            Current.CurrentSize = size;
        }

        /// <summary>
        /// Drawer wants to wipe canvas.
        /// </summary>
        [ConCmd.Server]
        public static void ClearCanvas()
        {
            if(!ClientUtil.CanDraw(ConsoleSystem.Caller))
                return;

            Sound.FromScreen("pagecrumbling");
            Current.Hud.ClearCanvas(To.Everyone);
        }

        [ConCmd.Admin("sketch_skipword", Help = "Skips current word/drawer.")]
        public static void SkipWord()
        {
            //Not currently drawing, error out.
            if(Current.CurrentState is not PlayingState)
            {
                Current.CommandError(To.Single(ConsoleSystem.Caller), "Sketch: Cannot skip word because nothing is being drawn (incorrect state).");
                return;
            }

            (Current.CurrentState as PlayingState).Skip();
        }

        [ConCmd.Admin("sketch_resetgame", Help = "Resets game.")]
        public static void ResetGame()
        {
            Current.CurrentDrawerIndex = 0;
            Current.CurRound = 1;
            Current.Hud.ClearCanvas(To.Everyone);
            Current.CurrentLetters.Clear();
            foreach(var c in Game.Clients)
                c.SetInt("GameScore", 0);

            Current.CurrentState = new WaitingForPlayersState();
        }
    }
}
