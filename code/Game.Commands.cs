﻿using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			Log.Error( errormessage );
		}

		[AdminCmd("sketch_maxrounds", Help = "How many rounds to play before returning to lobby." )]
		public static void SetMaxRounds(int maxrounds)
		{
			//Invalid maxrounds number, error out.
			if(maxrounds <= 0)
			{
				Current.CommandError( To.Single( ConsoleSystem.Caller ), "Sketch: Invalid MaxRounds value (must be greater than 0)." );
				return;
			}

			Current.MaxRounds = maxrounds;
		}

		[AdminCmd("sketch_selectwordtime", Help = "How long (SECONDS) the drawer has to select a word.")]
		public static void SetSelectWordTime(int time)
		{
			//Invalid time, error out.
			if(time < 0)
			{
				Current.CommandError( To.Single( ConsoleSystem.Caller ), "Sketch: Invalid SelectWordTime value (must be non-negative)." );
				return;
			}

			Current.SelectWordTime = time;
		}

		[AdminCmd( "sketch_playtime", Help = "How long (SECONDS) the drawer has to draw/players have to guess." )]
		public static void SetPlayTime( int time )
		{
			//Invalid time, error out.
			if ( time <= 0 )
			{
				Current.CommandError( To.Single( ConsoleSystem.Caller ), "Sketch: Invalid PlayTime value (must be greater than 0)." );
				return;
			}

			Current.PlayTime = time;
		}

		[AdminCmd( "sketch_wordpoolsize", Help = "How many words the drawer gets to choose from." )]
		public static void SetWordPoolSize( int size )
		{
			//Invalid time, error out.
			if ( size <= 0 )
			{
				Current.CommandError( To.Single( ConsoleSystem.Caller ), "Sketch: Invalid WordPoolSize value (must be greater than 0)." );
				return;
			}

			Current.WordPoolSize = size;
		}

		[AdminCmd( "sketch_setpixel", Help = "How many words the drawer gets to choose from." )]
		public static void SetWordPoolSize( int index, int r, int g, int b )
		{
			int[] newPixel = new int[] { index, r, g, b };
			Current.Hud.NetworkCanvasUpdate( newPixel );
		}
	}
}
