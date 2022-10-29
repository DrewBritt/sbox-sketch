using System;
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

            Texture2DBuilder build = Texture.Create(width, height).WithDynamicUsage();
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

        //Implementation of Bresenham's line algo, interpolates between points to draw w/o gaps xD
        public void DrawLine(int xStart, int yStart, int xEnd, int yEnd, Color32 color)
        {
            int xDist = Math.Abs(xEnd - xStart), xDir = xStart < xEnd ? 1 : -1;
            int yDist = Math.Abs(yEnd - yStart), yDir = yStart < yEnd ? 1 : -1;

            int err = (xDist > yDist ? xDist : -yDist) / 2, e = 0;

            for (;;)
            {
                if (xStart >= 0 && xStart < width && yStart >= 0 && yStart < height)
                {
                    //Calculate pixels in radius locally
                    var indices = FindPixelsInDistance(new Vector2((float) xStart, (float) yStart), Game.Current.CurrentSize);
                    foreach(var index in indices)
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
                }

                if (xStart == xEnd && yStart == yEnd)
                    break;

                e = err;
                if (e > -xDist)
                {
                    err -= (int) yDist;
                    xStart += (int) xDir;
                }
                if (e <  yDist)
                {
                    err += (int) xDist;
                    yStart += (int) yDir;
                }
            }
        }

        //Left click down + dragging mouse
        //TODO: Proper input probably? Too much of a dumbfuck to figure out
        //how to check AttackPrimary, UI is taking all the mouse events.
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

            Color32 color = Game.Current.CurrentColor;

            //Add panel position to send to other clients
            //We grab the last plotted point here so we can interpolate between it and the new point.
            //Gives us a smoother looking image.
            //Note that this is using NewPixelsPos which gets regularly cleared. When it gets cleared, there will be gaps in the drawing.
            //I'm too lazy to fix this, but if the gaps annoy you, there is your hint xD
            Vector2 oldPos = pos;
            if (NewPixelsPos.Count > 0)
                oldPos = NewPixelsPos[NewPixelsPos.Count - 1];
            NewPixelsPos.Add(pos);
            DrawLine((int) oldPos.x, (int) oldPos.y, (int) pos.x, (int) pos.y, color);

            //Calculate pixels in radius locally
            RedrawCanvas();
        }

        public List<int> FindPixelsInDistance(Vector2 pos, int radius)
        {
            List<int> indices = new List<int>();
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
                    indices.Add(index);
                }
            }

            return indices;
        }
    }
}
