using Sandbox;
using Sandbox.Hooks;
using Sandbox.UI;
using Sandbox.UI.Construct;

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

			StyleSheet.Load( "UI/ChatBox.scss" );

			Container = Add.Panel("container");

			Canvas = Container.Add.Panel( "chat_canvas" );
			Canvas.PreferScrollToBottom = true;

			Input = Container.Add.TextEntry( "" );
			Input.AddEventListener( "onsubmit", () => Submit() );
			Input.AcceptsFocus = true;
			Input.AllowEmojiReplace = true;

			Chat.OnOpenChat += FocusChat;
		}

		void FocusChat()
		{
			Input.Focus();

			Canvas.TryScrollToBottom();
		}

		void Submit()
		{
			var msg = Input.Text.Trim();
			Input.Text = "";

			if ( string.IsNullOrWhiteSpace( msg ) )
				return;

			Say( msg );

			Canvas.TryScrollToBottom();
		}

		public void AddEntry( string name, string message, string additionalClass = null )
		{
			var e = Canvas.AddChild<ChatEntry>();
			e.Message.Text = message;
			e.NameLabel.Text = name;

			e.SetClass( "noname", string.IsNullOrEmpty( name ) );

			if ( !string.IsNullOrEmpty( additionalClass ) )
				e.AddClass( additionalClass );

			Canvas.TryScrollToBottom();
		}

		[ClientCmd( "chat_add", CanBeCalledFromServer = true )]
		public static void AddChatEntry( string name, string message)
		{
			Current?.AddEntry( name, message );

			// Only log clientside if we're not the listen server host
			if ( !Global.IsListenServer )
			{
				Log.Info( $"{name}: {message}" );
			}
		}

		[ClientCmd( "chat_addinfo", CanBeCalledFromServer = true )]
		public static void AddInformation( string message, bool important = false )
		{
			Current?.AddEntry( null, message, important ? "information" : null );
		}

		[ServerCmd( "say" )]
		public static void Say( string message )
		{
			Assert.NotNull( ConsoleSystem.Caller );

			// todo - reject more stuff
			if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
				return;

			Log.Info( $"{ConsoleSystem.Caller}: {message}" );
			AddChatEntry( To.Everyone, ConsoleSystem.Caller.Name, message);
		}
	}

	public partial class ChatEntry : Panel
	{
		public Label NameLabel { get; internal set; }
		public Label Message { get; internal set; }

		public ChatEntry()
		{
			NameLabel = Add.Label( "Name", "name" );
			Message = Add.Label( "Message", "message" );
		}
	}
}
