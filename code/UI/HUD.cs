using Sandbox;
using Sandbox.UI;

namespace Sketch
{
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
            if(!IsClient)
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
            Sound.FromScreen(soundname);
        }

        /// <summary>
        /// Popup to display current drawer pre-round.
        /// </summary>
        [ClientRpc]
        public void DisplayCurrentDrawer()
        {
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
            SelectWord.Pool = pool;
            SelectWord.DisplayWordPool(time);
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
            StateInfo.BlankLetters.Text = currentletters;
        }

        /// <summary>
        /// Recreates a fresh canvas texture on the called client.
        /// </summary>
        [ClientRpc]
        public void ClearCanvas()
        {
            DrawCanvas.InitializeCanvasTexture();
        }

        /// <summary>
        /// Grabs updated pixel data and sends to server
        /// </summary>
        [ClientRpc]
        public void FetchDeltaCanvasData()
        {
            if(DrawCanvas.NewPixelsPos.Count == 0)
                return;

            string updatedPixels = "";
            foreach(var p in DrawCanvas.NewPixelsPos)
            {
                updatedPixels += $"{p},";
            }

            DrawCanvas.NewPixelsPos.Clear();
            Game.ReceiveDeltaCanvasData(updatedPixels);
        }

        [ClientRpc]
        public void UpdateGuessersCanvas(Vector2[] positions)
        {
            Color32 color = Game.Current.CurrentColor;
            foreach(var p in positions)
            {
                var indexes = DrawCanvas.FindPixelsInDistance(p, Game.Current.CurrentSize);
                foreach(var index in indexes)
                {
                    var pixel = new Pixel
                    {
                        Index = index,
                        Red = color.r,
                        Green = color.g,
                        Blue = color.b,
                    };
                    DrawCanvas.FillPixel(pixel);
                }
            }
            DrawCanvas.RedrawCanvas();
        }
    }
}
