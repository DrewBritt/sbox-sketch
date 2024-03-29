﻿using Sandbox;
using Sandbox.UI;
using Sketch.UI;

namespace Sketch.UI;

public partial class Hud : HudEntity<RootPanel>
{
    public StateInfo StateInfo { get; set; }
    public Panel GamePanel { get; set; }
    public Scoreboard Scoreboard { get; set; }
    public Panel DrawPanel { get; set; }
    public DrawCanvas DrawCanvas { get; set; }
    public ToolsContainer ToolsContainer { get; set; }
    public ChatBox ChatBox { get; set; }
    public SelectWord SelectWord { get; set; }
    public CurrentDrawer CurrentDrawer { get; set; }
    public Settings Settings { get; set; }

    public Hud()
    {
        if(!Game.IsClient)
            return;

        RootPanel.StyleSheet.Load("/ui/Hud.scss");
        StateInfo = RootPanel.AddChild<StateInfo>();

        GamePanel = RootPanel.Add.Panel("gamepanel");
        Scoreboard = GamePanel.AddChild<Scoreboard>();

        DrawPanel = GamePanel.Add.Panel("drawpanel");
        DrawCanvas = DrawPanel.AddChild<DrawCanvas>();
        ToolsContainer = DrawPanel.AddChild<ToolsContainer>();

        ChatBox = GamePanel.AddChild<ChatBox>();

        SelectWord = RootPanel.AddChild<SelectWord>();
        CurrentDrawer = RootPanel.AddChild<CurrentDrawer>();
        Settings = RootPanel.AddChild<Settings>();
    }

    /// <summary>
    /// Play generic .sound file on client
    /// </summary>
    /// <param name="soundname"></param>
    [ClientRpc]
    public new void PlaySound(string soundname)
    {
        Game.AssertClient();
        Sound.FromScreen(soundname);
    }

    /// <summary>
    /// Popup to display current drawer pre-round.
    /// </summary>
    [ClientRpc]
    public void DisplayCurrentDrawer()
    {
        Game.AssertClient();
        CurrentDrawer.DisplayCurrentDrawer();
    }

    /// <summary>
    /// Sends Word Pool to drawer to display elements on screen
    /// </summary>
    /// <param name="pool"></param>
    /// <param name="time"></param>
    [ClientRpc]
    public void SendWordPool(string[] pool, int time)
    {
        Game.AssertClient();
        SelectWord.DisplayWordPool(pool, time);
    }

    /// <summary>
    /// Sends current letters to client. 
    /// If guessers receive this, it's the currentletters with underscores.
    /// If drawer receives this, it's once at the beginning of the round with the word.
    /// </summary>
    /// <param name="currentletters"></param>
    [ClientRpc]
    public void SendCurrentLetters(string currentletters)
    {
        Game.AssertClient();
        StateInfo.Letters.Text = currentletters;
    }

    /// <summary>
    /// Recreates a fresh canvas texture on the called client.
    /// </summary>
    [ClientRpc]
    public void ClearCanvas()
    {
        Game.AssertClient();
        DrawCanvas.InitializeCanvasTexture();
    }

    /// <summary>
    /// Server is grabbing updated pixel data from drawer's (local) canvas.
    /// </summary>
    [ClientRpc]
    public void FetchDeltaCanvasData()
    {
        Game.AssertClient();

        if(DrawCanvas.NewPixelsPos.Count == 0)
            return;

        string updatedPixels = "";
        foreach(var p in DrawCanvas.NewPixelsPos)
        {
            updatedPixels += $"{p},";
        }

        DrawCanvas.NewPixelsPos.Clear();
        GameManager.ReceiveDeltaCanvasData(updatedPixels);
    }

    /// <summary>
    /// Server is sending "delta canvas data" to guesser clients.
    /// </summary>
    [ClientRpc]
    public void UpdateGuessersCanvas(Vector2[] positions)
    {
        Game.AssertClient();
        DrawCanvas.UpdateCanvas(positions);
    }

    [ClientRpc]
    public void EnableGameOverPanel()
    {
        Game.AssertClient();
        RootPanel.AddChild<GameOver>();
    }
}
