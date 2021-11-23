using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using System.Linq;

namespace Sketch
{
	public partial class Scoreboard : Panel
	{
		public Panel Container { get; protected set; }

		public Panel Canvas { get; protected set; }
		Dictionary<Client, ScoreboardEntry> Rows = new();

		public Scoreboard()
		{
			Container = Add.Panel( "container" );

			StyleSheet.Load( "UI/Scoreboard.scss" );
			Canvas = Container.Add.Panel( "canvas" );
		}

		public override void Tick()
		{

			// Add newly joined cliets
			foreach ( var client in Client.All.Except( Rows.Keys ) )
			{
				var entry = AddClient( client );
				Rows[client] = entry;
			}

			// Remove disconnected clients
			foreach ( var client in Rows.Keys.Except( Client.All ) )
			{
				if ( Rows.TryGetValue( client, out var row ) )
				{
					row?.Delete();
					Rows.Remove( client );
				}
			}
		}

		protected virtual ScoreboardEntry AddClient( Client entry )
		{
			var p = Canvas.AddChild<ScoreboardEntry>();
			p.Client = entry;
			return p;
		}
	}

	public partial class ScoreboardEntry : Panel
	{
		public Client Client;

		public Image Avatar { get; internal set; }
		public Label PlayerName { get; internal set; }
		public Label Score { get; internal set; }

		public ScoreboardEntry()
		{
			AddClass( "entry" );

			Avatar = Add.Image();
			PlayerName = Add.Label( "PlayerName", "name" );
			Score = Add.Label( "0000", "" );
		}

		RealTimeSince TimeSinceUpdate = 0;

		public override void Tick()
		{
			base.Tick();

			if ( !IsVisible )
				return;

			if ( !Client.IsValid() )
				return;

			if ( TimeSinceUpdate < 0.1f )
				return;

			TimeSinceUpdate = 0;
			UpdateData();
		}

		public virtual void UpdateData()
		{
			PlayerName.Text = Client.Name;
			//Score.Text = Client.GetInt("score"); DOESNT ACTUALLY WORK JUST WRITING FOR LATER ME
		}

		public virtual void UpdateFrom( Client client )
		{
			Client = client;
			UpdateData();
		}
	}
}
