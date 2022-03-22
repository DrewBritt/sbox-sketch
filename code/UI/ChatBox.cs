using Sandbox;
using Sandbox.Hooks;
using Sandbox.UI;
using Sandbox.UI.Construct;
using static Sketch.Game;

namespace Sketch
{
    [UseTemplate]
    public partial class ChatBox : Panel
    {
        public static ChatBox Current;

        public Panel Container { get; protected set; }
        public Panel Canvas { get; protected set; }
        public TextEntry Input { get; protected set; }

        public ChatBox()
        {
            Current = this;

            StyleSheet.Load("UI/ChatBox.scss");

            Container = Add.Panel("container");

            Canvas = Container.Add.Panel("chat_canvas");
            Canvas.PreferScrollToBottom = true;

            Input = Container.Add.TextEntry("");
            Input.Placeholder = "Enter your guess here...";
            Input.AddEventListener("onsubmit", () => Submit());
            Input.AddEventListener("onclick", () => FocusChat());
            Input.AcceptsFocus = true;
            Input.AllowEmojiReplace = true;

            Chat.OnOpenChat += FocusChat;
        }

        void FocusChat()
        {
            Input.Focus();
            Input.Placeholder = "";
            Input.CaretPosition = 0;

            Canvas.TryScrollToBottom();
        }

        void Submit()
        {
            var msg = Input.Text.Trim();
            Input.Text = "";
            Input.Placeholder = "Enter your guess here...";

            if (string.IsNullOrWhiteSpace(msg))
                return;

            Say(msg);

            Canvas.TryScrollToBottom();
        }

        public void AddEntry(ulong? id, string name, string message, string additionalClass = null)
        {
            var e = Canvas.AddChild<ChatEntry>();
            e.NameLabel.Text = name;
            e.Message.Text = message;

            if(id != null)
            {
                e.Avatar.SetTexture($"avatar:{id}");
            }

            e.SetClass("noname", string.IsNullOrEmpty(name));

            if (!string.IsNullOrEmpty(additionalClass))
                e.AddClass(additionalClass);

            Canvas.TryScrollToBottom();
        }

        [ClientCmd("chat_add", CanBeCalledFromServer = true)]
        public static void AddChatEntry(string playerid, string name, string message)
        {
            Current?.AddEntry(playerid.ToULong(), name, message);

            // Only log clientside if we're not the listen server host
            if (!Global.IsListenServer)
            {
                Log.Info($"{name}: {message}");
            }
        }

        [ClientCmd("chat_guessedchat", CanBeCalledFromServer = true)]
        public static void AddGuessedChatEntry(ulong playerid, string name, string message)
        {
            Current?.AddEntry(playerid, name, message, "guessedchat");
        }

        [ClientCmd("chat_drawerchat", CanBeCalledFromServer = true)]
        public static void AddDrawerChatEntry(ulong playerid, string name, string message)
        {
            Current?.AddEntry(playerid, name, message, "drawerchat");
        }

        [ClientCmd("chat_addinfo", CanBeCalledFromServer = true)]
        public static void AddInformation(ulong playerid, string message, bool important = false)
        {
            Current?.AddEntry(playerid, null, message, important ? "information" : null);
        }

        [ServerCmd("say")]
        public static void Say(string message)
        {
            Assert.NotNull(ConsoleSystem.Caller);

            // todo - reject more stuff
            if (message.Contains('\n') || message.Contains('\r'))
                return;

            //Log.Info($"{ConsoleSystem.Caller}: {message}");

            var game = Game.Current;

            //Checks should only be ran if currently playing game
            if (game.CurrentState is PlayingState)
            {
                //Highlight drawer's chat (also prevents them from guessing the word)
                if (ConsoleSystem.Caller == game.CurrentDrawer)
                {
                    AddDrawerChatEntry(To.Everyone, (ulong)ConsoleSystem.Caller.PlayerId, $"{ConsoleSystem.Caller.Name}:", message);
                    return;
                }

                //If players' have already guessed, send to private chat
                var guessed = game.GuessedPlayers;
                if (guessed.Contains(ConsoleSystem.Caller))
                {
                    AddGuessedChatEntry(To.Multiple(game.GuessedPlayers), (ulong)ConsoleSystem.Caller.PlayerId, $"{ConsoleSystem.Caller.Name}:", message);
                    return;
                }

                //Check first word from players' message to check if answer
                var words = message.Split(' ', System.StringSplitOptions.TrimEntries);

                //Found word
                if (words[0].ToLower() == game.CurrentWord.ToLower())
                {
                    AddInformation(To.Everyone, (ulong)ConsoleSystem.Caller.PlayerId, $"{ConsoleSystem.Caller.Name} has guessed the word!", true);
                    Sound.FromScreen("xylophonealert");
                    game.SetPlayerGuessed(ConsoleSystem.Caller);
                    return;
                }
            }

            Log.Info(ConsoleSystem.Caller.PlayerId);
            AddChatEntry(To.Everyone, ConsoleSystem.Caller.PlayerId.ToString(), $"{ConsoleSystem.Caller.Name}:", message);
        }
    }

    public partial class ChatEntry : Panel
    {
        public Image Avatar { get; internal set; }
        public Label NameLabel { get; internal set; }
        public Label Message { get; internal set; }

        public ChatEntry()
        {
            Avatar = Add.Image();
            NameLabel = Add.Label("Name", "name");
            Message = Add.Label("Message", "message");
        }
    }
}
