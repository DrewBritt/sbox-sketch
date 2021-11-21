using Sandbox;
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
			}

			Current.PlayTime = time;
		}
	}
}
