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
		public class BaseState
		{
			protected RealTimeSince stateStart;

			public BaseState()
			{
				stateStart = 0;
			}

			public virtual string StateName() => GetType().ToString();

			public virtual string StateTime()
			{
				var time = TimeSpan.FromSeconds( stateStart );
				return $"{time.Minutes:D2}:{time.Seconds:D2}";
			}

			public virtual void Tick() { }

			protected void SetState(BaseState state)
			{
				Current.CurrentState = state;
			}
		}

		public class WaitingForPlayersState : BaseState
		{
			public override string StateName() => "Waiting for Players";

			public override void Tick()
			{
				if (Client.All.Count > 1)
				{
					SetState( new SelectingWordState());
				}
			}
		}

		/// <summary>
		/// The drawer is selecting a word.
		/// If the drawer selects a word, or if SelectWordTime seconds have passed and no word is selected,
		/// leads into PlayingState() with selected or random word.
		/// </summary>
		public class SelectingWordState : BaseState
		{
			public override string StateName() => "Selecting Word";
			private RealTimeUntil stateEnds;

			public SelectingWordState()
			{
				stateEnds = Current.SelectWordTime;
			}

			public override string StateTime()
			{
				var time = TimeSpan.FromSeconds( stateEnds );
				return $"{time.Minutes:D2}:{time.Seconds:D2}";
			}

			public override void Tick()
			{
				if(stateEnds < 0)
				{
					SetState( new PlayingState() );
				}
			}
		}

		/// <summary>
		/// The drawer is drawing and players are guessing the word.
		/// Leads into PostPlayingState after stateEnds is less than 0.
		/// </summary>
		public class PlayingState : BaseState
		{
			public override string StateName() => "Playing";
			private RealTimeUntil stateEnds;

			public PlayingState()
			{
				stateEnds = Current.PlayTime;
			}

			public override string StateTime()
			{
				var time = TimeSpan.FromSeconds( stateEnds );
				return $"{time.Minutes:D2}:{time.Seconds:D2}";
			}

			public override void Tick()
			{
				if(stateEnds < 0)
				{
					SetState( new PostPlayingState() );
				}
			}
		}

		/// <summary>
		/// Post-playing waiting state.
		/// Leads into SelectingWordState if CurrentPlayerIndex + 1 less than Client.All.Count,
		/// else leads into PostRoundState.
		/// </summary>
		public class PostPlayingState : BaseState
		{
			public override string StateName() => "Post-Drawing";
			private RealTimeUntil stateEnds;

			public PostPlayingState()
			{
				stateEnds = 8;
			}

			public override string StateTime()
			{
				var time = TimeSpan.FromSeconds( stateEnds );
				return $"{time.Minutes:D2}:{time.Seconds:D2}";
			}

			public override void Tick()
			{
			}
		}

		/// <summary>
		/// Post-round waiting state.
		/// A round concludes if the entire client list has been incremented over.
		/// If curRound is less than or equal to MaxRounds, starts new round (leads into SelectingWordState).
		/// Otherwise, leads into PostGameState
		/// </summary>
		public class PostRoundState : BaseState
		{
			public override string StateName() => "Post-Round";
			private RealTimeUntil stateEnds;

			public PostRoundState()
			{
				stateEnds = 10;
			}

			public override string StateTime()
			{
				var time = TimeSpan.FromSeconds( stateEnds );
				return $"{time.Minutes:D2}:{time.Seconds:D2}";
			}

			public override void Tick()
			{
				if(stateEnds < 0)
				{
					if(Current.CurRound < Current.MaxRounds)
					{
						Current.CurRound++;
						Current.CurrentPlayerIndex = 0;
						SetState( new SelectingWordState() );
					} else
					{
						SetState( new PostGameState() );
					}
				}
			}
		}

		/// <summary>
		/// Post-game waiting state.
		/// Kicks all players to menu after stateEnds is less than 0.
		/// </summary>
		public class PostGameState : BaseState
		{
			public override string StateName() => "Post-Game";
			private RealTimeUntil stateEnds;

			public PostGameState()
			{
				stateEnds = 10;
			}

			public override string StateTime()
			{
				var time = TimeSpan.FromSeconds( stateEnds );
				return $"{time.Minutes:D2}:{time.Seconds:D2}";
			}

			public override void Tick()
			{
				if(stateEnds < 0)
				{
					Client.All.ToList().ForEach( cl => cl.Kick() );
				}
			}
		}

		/// <summary>
		/// Only use on server, as only the properties are networked.
		/// </summary>
		public BaseState CurrentState { get; set; } = new WaitingForPlayersState();

		[Net] public string CurrentStateName { get; set; }
		[Net] public string CurrentStateTime { get; set; }

		/// <summary>
		/// How many rounds to play before returning to lobby.
		/// Set by sketch_maxrounds command.
		/// </summary>
		[Net] public int MaxRounds { get; set; } = 3;
		[Net] public int CurRound { get; set; } = 1;

		/// <summary>
		/// Keeps track of drawer in Client.All
		/// TODO: See if switching to custom player list (pawns) is better.
		/// </summary>
		public int CurrentPlayerIndex = 0;

		/// <summary>
		/// How long the drawer has to pick a word. 
		/// Set by sketch_selectwordtime command. Set to 0 for random words.
		/// </summary>
		public int SelectWordTime { get; set; } = 20;

		/// <summary>
		/// How long to draw/guess before selecting next drawer. Set by sketch_playtime command.
		/// </summary>
		public int PlayTime { get; set; } = 180;

		/// <summary>
		/// Current word to draw/guess. Not networked to avoid cheating, passed to drawer through ClientRPC.
		/// </summary>
		public string CurrentWord { get; set; }

		[Event.Tick]
		public void OnTick()
		{
			if ( Host.IsClient ) return;

			CurrentStateName = CurrentState.StateName();
			CurrentStateTime = CurrentState.StateTime();
			CurrentState.Tick();
		}
	}
}
