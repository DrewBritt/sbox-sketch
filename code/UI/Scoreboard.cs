using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Sketch
{
    public partial class Scoreboard : Panel
    {
        public Panel Container { get; protected set; }

        public Panel Canvas { get; protected set; }
        Dictionary<Client, ScoreboardEntry> Rows = new();

        public Scoreboard()
        {
            Container = Add.Panel("container");

            StyleSheet.Load("UI/Scoreboard.scss");
            Canvas = Container.Add.Panel("canvas");
        }

        public override void Tick()
        {
            // Add newly joined cliets
            foreach(var client in Client.All.Except(Rows.Keys))
            {
                var entry = AddClient(client);
                Rows[client] = entry;
            }

            // Remove disconnected clients
            foreach(var client in Rows.Keys.Except(Client.All))
            {
                if(Rows.TryGetValue(client, out var row))
                {
                    row?.Delete();
                    Rows.Remove(client);
                }
            }

            //Hacky way to sort scoreboard by playerscore without rewriting the scoreboard
            //TODO: Make this not hacky probably
            Canvas.SortChildren(p => (p as ScoreboardEntry).Client.GetInt("GameScore") * -1);
        }

        protected virtual ScoreboardEntry AddClient(Client cl)
        {
            var entry = new ScoreboardEntry(cl);
            Canvas.AddChild(entry);
            return entry;
        }

        public void OnVoicePlayed(long steamId, float level)
        {
            var playerEntry = ChildrenOfType<ScoreboardEntry>().FirstOrDefault(x => x.Client.PlayerId == steamId);
            if(playerEntry == null) return;

            playerEntry.UpdateVoice();
        }
    }

    public partial class ScoreboardEntry : Panel
    {
        public Client Client;
        readonly Image Avatar;
        readonly Label PlayerName;
        public Label Score { get; internal set; }

        private int lastIndex;

        public ScoreboardEntry(Client cl)
        {
            Client = cl;
            AddClass("entry");

            Avatar = Add.Image($"avatar:{Client.PlayerId}");
            PlayerName = Add.Label(Client.Name, "name");
            Score = Add.Label("0000", "score");
        }

        RealTimeSince TimeSinceUpdate = 0;

        public override void Tick()
        {
            base.Tick();

            if(!IsVisible)
                return;

            if(!Client.IsValid())
                return;

            if(TimeSinceUpdate < 0.1f)
                return;

            TimeSinceUpdate = 0;
            UpdateData();
        }

        public virtual void UpdateData()
        {
            Avatar.SetTexture($"avatar:{Client.PlayerId}");

            var name = Client.Name;
            if(Game.Current.CurrentDrawer == Client)
                name += "✏️";

            if(timeSinceVoicePlayed < 2)
                name += " 🔊";

            PlayerName.Text = name;

            Score.Text = Client.GetInt("GameScore").ToString();

            //Update stylings based on rank
            if(SiblingIndex != lastIndex)
            {
                lastIndex = SiblingIndex;
                RemoveClass("first");
                RemoveClass("second");
                RemoveClass("third");

                if(SiblingIndex == 1)
                    AddClass("first");
                if(SiblingIndex == 2)
                    AddClass("second");
                if(SiblingIndex == 3)
                    AddClass("third");
            }
        }

        public virtual void UpdateFrom(Client client)
        {
            Client = client;
            UpdateData();
        }

        TimeSince timeSinceVoicePlayed = 5;
        public void UpdateVoice()
        {
            timeSinceVoicePlayed = 0;
        }
    }
}
