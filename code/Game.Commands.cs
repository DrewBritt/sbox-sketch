using System;
using Sandbox;

namespace Sketch
{
    public partial class Game
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
        [ServerCmd]
        public static void SetCurrentColor(string rgba)
        {
            if (ConsoleSystem.Caller != Current.CurrentDrawer)
                return;

            //Split rgba string "rgba( r, g, b, a )" into [r, g, b, a]
            //Substring cuts off "rgba( " and IndexOf gets the final parenthesis
            string[] bytes = rgba.Substring(6, rgba.IndexOf(')') - 6).Split(',', StringSplitOptions.TrimEntries);

            Color32 newColor = new Color32((byte)bytes[0].ToInt(), (byte)bytes[1].ToInt(), (byte)bytes[2].ToInt());
            Current.CurrentColor = newColor;
        }

        [ServerCmd]
        public static void SetCurrentSize(int size)
        {
            if (ConsoleSystem.Caller != Current.CurrentDrawer)
                return;

            Current.CurrentSize = size;
        }

        [ServerCmd]
        public static void ClearCanvas()
        {
            if (ConsoleSystem.Caller != Current.CurrentDrawer)
                return;

            Current.Hud.ClearCanvas(To.Everyone);
        }

        [AdminCmd("sketch_maxrounds", Help = "How many rounds to play before returning to lobby.")]
        public static void SetMaxRounds(int maxrounds)
        {
            //Invalid maxrounds number, error out.
            if (maxrounds <= 0)
            {
                Current.CommandError(To.Single(ConsoleSystem.Caller), "Sketch: Invalid MaxRounds value (must be greater than 0).");
                return;
            }

            Current.MaxRounds = maxrounds;
        }

        [AdminCmd("sketch_selectwordtime", Help = "How long (SECONDS) the drawer has to select a word.")]
        public static void SetSelectWordTime(int time)
        {
            //Invalid time, error out.
            if (time < 0)
            {
                Current.CommandError(To.Single(ConsoleSystem.Caller), "Sketch: Invalid SelectWordTime value (must be non-negative).");
                return;
            }

            Current.SelectWordTime = time;
        }

        [AdminCmd("sketch_playtime", Help = "How long (SECONDS) the drawer has to draw/players have to guess.")]
        public static void SetPlayTime(int time)
        {
            //Invalid time, error out.
            if (time <= 0)
            {
                Current.CommandError(To.Single(ConsoleSystem.Caller), "Sketch: Invalid PlayTime value (must be greater than 0).");
                return;
            }

            Current.PlayTime = time;
        }

        [AdminCmd("sketch_wordpoolsize", Help = "How many words the drawer gets to choose from.")]
        public static void SetWordPoolSize(int size)
        {
            //Invalid time, error out.
            if (size <= 0)
            {
                Current.CommandError(To.Single(ConsoleSystem.Caller), "Sketch: Invalid WordPoolSize value (must be greater than 0).");
                return;
            }

            Current.WordPoolSize = size;
        }

        [AdminCmd("sketch_skipword", Help = "Skips current word/drawer.")]
        public static void SkipWord()
        {
            //Not currently drawing, error out.
            if (Current.CurrentState is not PlayingState)
            {
                Current.CommandError(To.Single(ConsoleSystem.Caller), "Sketch: Cannot skip word because nothing is being drawn (incorrect state).");
                return;
            }

            (Current.CurrentState as PlayingState).Skip();
        }
    }
}
