using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;

namespace Sketch
{
    public struct Pixel
    {
        public int Index;
        public byte Red, Green, Blue;

        public override string ToString() => $"{Index} {Red} {Green} {Blue} ";

        public Pixel(string pixel)
        {
            var data = pixel.Split(',');
            Index = data[0].ToInt();
            Red = (byte)data[1].ToInt();
            Green = (byte)data[2].ToInt();
            Blue = (byte)data[3].ToInt();
        }
    }

    public partial class DrawCanvas : Image
    {
        int width = 800;
        int height = 600;

        /// <summary>
        /// RGBA data for Canvas texture.
        /// </summary>
        public byte[] CanvasInfo;

        public IList<Pixel> UpdatedPixels { get; set; } = new List<Pixel>();
        public DrawCanvas()
        {
            StyleSheet.Load("UI/DrawCanvas.scss");

            InitializeCanvasTexture();
            Style.Height = height;
            Style.Width = width;
        }

        public void InitializeCanvasTexture()
        {
            CanvasInfo = new byte[width * height * 4];
            for (int i = 0; i < CanvasInfo.Length; i++)
                CanvasInfo[i] = 255;

            Texture2DBuilder build = Texture.Create(width, height);
            build.WithData(CanvasInfo, CanvasInfo.Length);
            Texture = build.Finish();
        }

        public void RedrawCanvas() => Texture.Update(CanvasInfo);

        public void UpdateCanvasInfo(Pixel pixel)
        {
            CanvasInfo[pixel.Index] = pixel.Red;
            CanvasInfo[pixel.Index + 1] = pixel.Green;
            CanvasInfo[pixel.Index + 2] = pixel.Blue;


            if (Client.All[Game.Current.CurrentDrawerIndex] == Local.Client)
                UpdatedPixels.Add(pixel);
        }

        //Left click down + dragging mouse
        //TODO: Proper input probably? Too much of a dumbfuck to figure out
        //how to check Attack1, UI is taking all the mouse events.
        protected override void OnDragSelect(SelectionEvent e)
        {
            if (Client.All[Game.Current.CurrentDrawerIndex] != Local.Client)
                return;

            var pos = ScreenPositionToPanelPosition(e.EndPoint);
            if (pos.x < 0 || pos.y < 0 || pos.x > width || pos.y > height)
                return;
            Log.Info($"MousePos: {Mouse.Position}, MousePosToPanelPos: {pos}");

            var indexes = FindPixelsInDistance(pos, 1);
            foreach (var index in indexes)
            {
                var p = new Pixel
                {
                    Index = index,
                    Red = 255,
                    Green = 0,
                    Blue = 0,
                };
                UpdateCanvasInfo(p);
            }
            RedrawCanvas();
        }

        public List<int> FindPixelsInDistance(Vector2 pos, int radius)
        {
            List<int> indexes = new List<int>();
            int xpos = (int)pos.x;
            int ypos = (int)pos.y;

            for (int x = xpos - radius; x <= xpos + radius; x++)
            {
                //Check for wrapping
                if (x < 0 || x >= width)
                    continue;

                for (int y = ypos - radius; y <= ypos + radius; y++)
                {
                    //Check for wrapping
                    if (y < 0 || y >= height)
                        continue;

                    //Pixel found, calculate and add index
                    int index = (y * width * 4) + (x * 4);
                    indexes.Add(index);
                }
            }

            return indexes;
        }
    }
}
