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

    /// <summary>
    /// Canvas for drawer to paint their image onto
    /// </summary>
    public partial class DrawCanvas : Image
    {
        int width = 800;
        int height = 600;

        /// <summary>
        /// RGBA data for Canvas texture.
        /// </summary>
        public byte[] CanvasInfo;

        /// <summary>
        /// Newly drawn pixels have their panel pos sent over the server and calculated per-client,
        /// to optimize handling color/radius data (instead of sending it every pixel)
        /// </summary>
        public IList<Vector2> NewPixelsPos { get; set; } = new List<Vector2>();
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
            for(int i = 0; i < CanvasInfo.Length; i++)
                CanvasInfo[i] = 255;

            Texture2DBuilder build = Texture.Create(width, height);
            build.WithData(CanvasInfo, CanvasInfo.Length);
            Texture = build.Finish();
        }

        public void RedrawCanvas() => Texture.Update(CanvasInfo);

        public void FillPixel(Pixel newPixel)
        {
            CanvasInfo[newPixel.Index] = newPixel.Red;
            CanvasInfo[newPixel.Index + 1] = newPixel.Green;
            CanvasInfo[newPixel.Index + 2] = newPixel.Blue;
        }

        //Left click down + dragging mouse
        //TODO: Proper input probably? Too much of a dumbfuck to figure out
        //how to check Attack1, UI is taking all the mouse events.
        protected override void OnDragSelect(SelectionEvent e)
        {
            //Not current drawer, block drawing
            if(!ClientUtil.CanDraw(Local.Client))
                return;

            //Get panel pos
            //TODO: FIX FIX FIX FIX FIX
            //MousePos -> PanelPos was so off before, ScaleFromScreen almost fixed it, and .95 is a random hack number.
            //I think it's because of how the UI is internally handled (scales to 1920x1080 or some shit)
            //Still doesn't work completely properly on other resolutions, just looks good enough for now :(
            var pos = MousePosition * ScaleFromScreen * .95;
            if(pos.x < 0 || pos.y < 0 || pos.x > width || pos.y > height)
                return;

            //Add panel position to send to other clients
            NewPixelsPos.Add(pos);

            //Calculate pixels in radius locally
            var indexes = FindPixelsInDistance(pos, Game.Current.CurrentSize);
            Color32 color = Game.Current.CurrentColor;
            foreach(var index in indexes)
            {
                var p = new Pixel
                {
                    Index = index,
                    Red = color.r,
                    Green = color.g,
                    Blue = color.b,
                };
                FillPixel(p);
            }
            RedrawCanvas();
        }

        public List<int> FindPixelsInDistance(Vector2 pos, int radius)
        {
            List<int> indexes = new List<int>();
            int xpos = (int)pos.x;
            int ypos = (int)pos.y;

            for(int x = xpos - radius; x <= xpos + radius; x++)
            {
                //Check for wrapping
                if(x < 0 || x >= width)
                    continue;

                for(int y = ypos - radius; y <= ypos + radius; y++)
                {
                    //Check for wrapping
                    if(y < 0 || y >= height)
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
