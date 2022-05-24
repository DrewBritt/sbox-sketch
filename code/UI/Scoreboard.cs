﻿using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Sketch;
public partial class Scoreboard : Panel
{
    public Panel Container { get; protected set; }

    public Panel EntryList { get; protected set; }
    Dictionary<Client, ScoreboardEntry> Rows = new();

    public bool Dirty = true;

    public Scoreboard()
    {
        Container = Add.Panel("container");

        StyleSheet.Load("UI/Scoreboard.scss");
        EntryList = Container.Add.Panel("entrylist");
    }

    public override void Tick()
    {
        if(!Dirty)
            return;

        //Add newly joined clients
        foreach(var client in Client.All.Except(Rows.Keys))
        {
            var entry = AddClient(client);
            Rows[client] = entry;
        }

        //Remove disconnected clients
        foreach(var client in Rows.Keys.Except(Client.All))
        {
            if(Rows.TryGetValue(client, out var row))
            {
                row?.Delete();
                Rows.Remove(client);
            }
        }

        EntryList.SortChildren(p => (p as ScoreboardEntry).Client.GetInt("GameScore") * -1);
        Dirty = false;
    }

    protected virtual ScoreboardEntry AddClient(Client cl)
    {
        var entry = new ScoreboardEntry(cl);
        EntryList.AddChild(entry);
        return entry;
    }
}

public partial class ScoreboardEntry : Panel
{
    public Client Client;
    readonly Label IsDrawing;
    public Label Score { get; internal set; }

    public ScoreboardEntry(Client cl)
    {
        Client = cl;
        AddClass("entry");

        Add.Image($"avatar:{Client.PlayerId}");
        Add.Label(Client.Name);

        IsDrawing = Add.Label("✏️", "isdrawing");
        IsDrawing.BindClass("enable", () => Game.Current.CurrentDrawer == Client);

        Score = Add.Label("0", "score");
        Score.Bind("text", () => Client.GetInt("GameScore").ToString());

        BindClass("first", () => SiblingIndex == 1);
        BindClass("second", () => SiblingIndex == 2);
        BindClass("third", () => SiblingIndex == 3);
    }
}
