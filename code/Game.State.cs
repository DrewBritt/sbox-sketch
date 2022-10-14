using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Sketch;

public partial class Game
{
    #region States
    public class BaseState
    {
        public RealTimeSince stateStart { get; }

        public BaseState()
        {
            stateStart = 0;
        }

        public virtual string StateName() => GetType().ToString();

        public virtual string StateTime()
        {
            var time = TimeSpan.FromSeconds(stateStart);
            return $"{time.Minutes:D2}:{time.Seconds:D2}";
        }

        public virtual void Tick()
        {
            if(Client.All.Count == 1)
                SetState(new WaitingForPlayersState());

            if(Current.CurrentDrawer != null && !Current.CurrentDrawer.IsValid())
            {
                Current.CurrentDrawerIndex--;
                Current.CurrentDrawer = null;
                SetState(new PostPlayingState());
            }
        }

        protected void SetState(BaseState state)
        {
            Current.GuessedPlayers.Clear();
            Current.CurrentState = state;
        }
    }

    public class WaitingForPlayersState : BaseState
    {
        public override string StateName() => "Waiting for Players";

        public WaitingForPlayersState() : base()
        {
        }

        public override void Tick()
        {
            if(Client.All.Count > 1)
            {
                SetState(new PreSelectingState());
            }
        }
    }

    /// <summary>
    /// The current drawer is displayed on screen.
    /// Used to ensure UI appears on screen without conflicting with game settings.
    /// EX: Setting SelectWordTime to 0 for random words.
    /// </summary>
    public class PreSelectingState : BaseState
    {
        public override string StateName() => "Selecting Word";
        private RealTimeUntil stateEnds = 3;

        public PreSelectingState() : base()
        {
            Current.CurrentDrawer = Client.All[Current.CurrentDrawerIndex];
            Current.Hud.DisplayCurrentDrawer(To.Everyone);
        }

        public override string StateTime()
        {
            var time = TimeSpan.FromSeconds(stateEnds);
            return $"{time.Minutes:D2}:{time.Seconds:D2}";
        }

        public override void Tick()
        {
            base.Tick();

            if(stateEnds < 0)
            {
                SetState(new SelectingWordState());
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

        public string[] WordPool;

        public SelectingWordState() : base()
        {
            //Send word pool to drawer.
            WordPool = Words.RandomWords(WordPoolSize);
            Current.Hud.SendWordPool(To.Single(Current.CurrentDrawer), WordPool.ToArray(), SelectWordTime);

            //Set random word ahead of time. If drawer doesn't select a word, this word is used.
            var random = new Random();
            int ranNum = random.Next(WordPool.Length);
            Current.CurrentWord = WordPool[ranNum];

            Current.CurrentLetters.Clear();

            stateEnds = SelectWordTime;
        }

        public override string StateTime()
        {
            var time = TimeSpan.FromSeconds(stateEnds);
            return $"{time.Minutes:D2}:{time.Seconds:D2}";
        }

        /// <summary>
        /// Stores word and progresses to PlayingState().
        /// </summary>
        /// <param name="word"></param>
        public void SelectWord(string word)
        {
            Current.CurrentWord = word;
            stateEnds = -1;
        }

        public override void Tick()
        {
            base.Tick();

            if(stateEnds < 0)
            {
                SetState(new PlayingState());
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
        private RealTimeUntil newLetter;

        public PlayingState() : base()
        {
            //Begin play sound
            Sound.FromScreen("bellshort");

            //Init CurrentLetters with empty spaces
            var word = Current.CurrentWord;
            var chars = new List<char>();
            for(int i = 0; i < Current.CurrentWord.Length; i++)
            {
                chars.Add('_');
            }
            Current.CurrentLetters = chars;

            stateEnds = DrawTime;
            newLetter = DrawTime / Current.CurrentWord.Length;

            //Send entire word to drawer
            Current.Hud.SendCurrentLetters(To.Single(Current.CurrentDrawer), word);

            //Only send current letters (at this point, just _'s) to everyone else
            var tosend = ClientUtil.ClientsExceptDrawer(Client.All, Current.CurrentDrawer);
            Current.Hud.SendCurrentLetters(To.Multiple(tosend), Current.CurrentLettersString());
        }

        public override string StateTime()
        {
            var time = TimeSpan.FromSeconds(stateEnds);
            return $"{time.Minutes:D2}:{time.Seconds:D2}";
        }


        TimeSince timeSinceCanvasUpdated = 0;
        Sound countdownSound, warningSound;
        public override void Tick()
        {
            base.Tick();

            //Fetch delta canvas data (1 / timeSinceCanvasUpdated) times a second
            if(timeSinceCanvasUpdated > .05)
            {
                timeSinceCanvasUpdated = 0;
                Current.Hud.FetchDeltaCanvasData(To.Single(Current.CurrentDrawer));
            }

            //Send new hint to guessers
            if(newLetter < 0)
            {
                //Select random letter and stick into char array
                var random = new Random();
                int ranNum = random.Next(Current.CurrentWord.Length);
                Current.CurrentLetters[ranNum] = Current.CurrentWord[ranNum];

                //Update only guesser's UI
                //TODO: Probably remove this garbage function. Set bool on drawer client and only update if false,
                //then send letters to everyone instead?
                var tosend = ClientUtil.ClientsExceptDrawer(Client.All, Current.CurrentDrawer);
                Current.Hud.SendCurrentLetters(To.Multiple(tosend), Current.CurrentLettersString());

                //Reset letter timer
                newLetter = DrawTime / Current.CurrentWord.Length;
            }

            //Warning sound shit
            if(stateEnds <= 30 && stateEnds >= 29 && warningSound.Finished)
                warningSound = Sound.FromScreen("maracashort");

            //Countdown sound shit
            if(stateEnds <= 3 && stateEnds > 0 && countdownSound.Finished)
                countdownSound = Sound.FromScreen("pingshort");

            if(stateEnds < 0)
            {
                Sound.FromScreen("belllong");
                SetState(new PostPlayingState());
            }
        }

        public void Skip()
        {
            stateEnds = 0;
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

        public PostPlayingState() : base()
        {
            stateEnds = 8;
            Current.CurrentLetters = Current.CurrentWord.ToList();
            Current.Hud.SendCurrentLetters(To.Everyone, Current.CurrentLettersString());
        }

        public override string StateTime()
        {
            var time = TimeSpan.FromSeconds(stateEnds);
            return $"{time.Minutes:D2}:{time.Seconds:D2}";
        }

        public override void Tick()
        {
            base.Tick();

            if(stateEnds < 0)
            {
                //Clear canvas/word data
                Current.Hud.ClearCanvas(To.Everyone);
                Current.Hud.SendCurrentLetters(To.Everyone, "");

                //New drawer
                if(Current.CurrentDrawerIndex != Client.All.Count - 1)
                {
                    Current.CurrentDrawerIndex++;
                    SetState(new PreSelectingState());
                }
                else
                {
                    SetState(new PostRoundState());
                }
            }
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

        public PostRoundState() : base()
        {
            stateEnds = 10;
            Current.CurrentLetters.Clear();
        }

        public override string StateTime()
        {
            var time = TimeSpan.FromSeconds(stateEnds);
            return $"{time.Minutes:D2}:{time.Seconds:D2}";
        }

        public override void Tick()
        {
            if(stateEnds < 0)
            {
                //Start new round
                if(Current.CurRound < MaxRounds)
                {
                    Current.CurRound++;

                    Current.CurrentDrawerIndex = 0;
                    SetState(new PreSelectingState());
                }
                else
                {
                    SetState(new PostGameState());
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

        public PostGameState() : base()
        {
            stateEnds = 10;
            Current.Hud.EnableGameOverPanel(To.Everyone);
            Current.CurrentDrawer = null;
        }

        public override string StateTime()
        {
            var time = TimeSpan.FromSeconds(stateEnds);
            return $"{time.Minutes:D2}:{time.Seconds:D2}";
        }

        public override void Tick()
        {
            if(stateEnds < 0)
            {
                Client.All.ToList().ForEach(cl => cl.Kick());
            }
        }
    }
    #endregion

    #region State Management
    /// <summary>
    /// Only use on server, as only the properties are networked.
    /// </summary>
    public BaseState CurrentState { get; set; } = new WaitingForPlayersState();

    [Net] public string CurrentStateName { get; set; }
    [Net] public string CurrentStateTime { get; set; }

    [Event.Tick]
    public void OnTick()
    {
        if(Host.IsClient) return;

        CurrentStateName = CurrentState.StateName();
        CurrentStateTime = CurrentState.StateTime();
        CurrentState.Tick();
    }

    /// <summary>
    /// How many rounds to play before returning to lobby.
    /// </summary>
    [ConVar.Replicated("sketch_maxrounds", Help = "How many rounds to play before returning to lobby.", Min = 1, Max = 10)]
    public static int MaxRounds { get; set; }
    [Net] public int CurRound { get; set; } = 1;

    /// <summary>
    /// How long to draw/guess before selecting next drawer.
    /// </summary>
    [ConVar.Replicated("sketch_drawtime", Help = "How long players have to draw/guess.", Min = 5, Max = 180)]
    public static int DrawTime { get; set; }

    [Net] public Client CurrentDrawer { get; set; }
    public int CurrentDrawerIndex = 0;
    #endregion

    #region Word Management
    /// <summary>
    /// How many words the drawer gets to choose from.
    /// </summary>
    [ConVar.Replicated("sketch_wordpoolsize", Help = "How many words the drawer can choose from.", Min = 1, Max = 5)]
    public static int WordPoolSize { get; set; }

    /// <summary>
    /// How long the drawer has to pick a word. 0 = random word selected.
    /// </summary>
    [ConVar.Replicated("sketch_selectwordtime", Help = "How much time the drawer has to choose a word", Min = 0, Max = 60)]
    public static int SelectWordTime { get; set; }

    /// <summary>
    /// Current word to draw/guess. Not networked to avoid cheating, passed to drawer through ClientRPC.
    /// </summary>
    public string CurrentWord { get; set; }

    /// <summary>
    /// Current letters displayed to non-drawers.
    /// </summary>
    public List<char> CurrentLetters { get; set; } = new List<char>();

    /// <summary>
    /// Nicer way to turn CurrentLetters into a string instead of writing a loop every fucking time.
    /// </summary>
    public string CurrentLettersString()
    {
        var w = "";
        foreach(var c in CurrentLetters)
            w += c;

        return w;
    }

    /// <summary>
    /// Drawer selects a word to draw.
    /// </summary>
    /// <param name="word">Word to draw/guess</param>
    [ConCmd.Server]
    public static void SelectWord(string word)
    {
        //Verify if command caller is the current drawer
        if(ConsoleSystem.Caller != Current.CurrentDrawer)
        {
            Current.CommandError(To.Single(ConsoleSystem.Caller), "Sketch: You're not the drawer!");
            return;
        }

        //Verify if state is proper
        if(Current.CurrentState is SelectingWordState state)
        {

            //And if selected word is valid
            if(state.WordPool.Contains(word))
            {
                state.SelectWord(word);
                return;
            }

            Current.CommandError(To.Single(ConsoleSystem.Caller), "Sketch: Selected word is not in word pool.");
            return;

        }

        Current.CommandError(To.Single(ConsoleSystem.Caller), "Sketch: Game is not in proper state!");
    }
    #endregion

    #region Drawing Management
    /// <summary>
    /// Drawer's currently selected color.
    /// </summary>
    [Net] public Color32 CurrentColor { get; set; }
    /// <summary>
    /// Drawer's currently selected brush size.
    /// </summary>
    [Net] public int CurrentSize { get; set; } = 6;

    /// <summary>
    /// Holds players that have guessed the current word.
    /// </summary>
    [Net] public List<Client> GuessedPlayers { get; set; } = new List<Client>();

    /// <summary>
    /// Player successfully guessed the word.
    /// </summary>
    /// <param name="cl"></param>
    public void SetPlayerGuessed(Client cl)
    {
        //Player score is calculated by lerping between 400 and 1000, 
        //Delta is higher if player is faster
        float delta = 1 - (CurrentState.stateStart / DrawTime);
        float score = MathX.LerpTo(400, 1000, delta);
        int curScore = cl.GetInt("GameScore");
        cl.SetInt("GameScore", curScore + score.FloorToInt());

        GuessedPlayers.Add(cl);

        //Drawer gets 2/3 of player score added to their own
        var drawer = Current.CurrentDrawer;
        curScore = drawer.GetInt("GameScore");
        drawer.SetInt("GameScore", curScore + (score.FloorToInt() / 3) * 2);

        Hud.SetScoreboardDirty(To.Everyone);

        //Check if all players have guessed (rather than checking every tick in state code)
        //TODO: Make this stupid shit not shit probably
        if(ClientUtil.ClientsExceptDrawer(Client.All, CurrentDrawer).SequenceEqual(GuessedPlayers))
        {
            Current.CurrentState = new PostPlayingState();
        }
    }

    /// <summary>
    /// Drawing client sends delta pixel data to server to resend to other clients.
    /// </summary>
    /// <param name="posData">A string of comma delimited MousePos x and y coordinates.</param>
    [ConCmd.Server]
    public static void ReceiveDeltaCanvasData(string posData)
    {
        var data = posData.Split(',', StringSplitOptions.TrimEntries);
        Vector2[] positions = new Vector2[posData.Length / 2];
        for(int i = 0; i < data.Length - 1; i += 2)
        {
            positions[i / 2] = new Vector2(data[i].ToInt(), data[i + 1].ToInt());
        }

        var tosend = ClientUtil.ClientsExceptDrawer(Client.All, Current.CurrentDrawer);
        Current.Hud.UpdateGuessersCanvas(To.Multiple(tosend), positions);
    }
    #endregion
}
