using System;
using System.Collections.Generic;
using Sandbox;
using System.Linq;

namespace Sketch
{
	/// <summary>
	/// Handles random words for the players to draw.
	/// </summary>
	public static class Words
	{
		public static List<string> WordList;
		public static string ListPath = "Util/wordlist.txt";

		public static void InitWordList()
		{
			if(FileSystem.Mounted.FileExists(ListPath))
			{
				WordList = FileSystem.Mounted.ReadAllText( ListPath ).Split( '\n' ).ToList();
				return;
			}

			Log.Error( "Sketch: WordList not loaded." );
		}
	}
}
