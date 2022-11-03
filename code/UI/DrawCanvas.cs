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
    /// Canvas for drawer to paint their image onto.
    /// </summary>
    public partial class DrawCanvas : Image
    {
        private int width = 800;
        private int height = 600;

        /// <summary>
        /// RGBA data for Canvas texture.
        /// </summary>
        private byte[] CanvasInfo;

        /// <summary> 
        /// The panel position at the drawer's mouse is captured each time they "draw", and these
        /// are what is sent to other clients, where each surrounding pixel is calculated locally,
        /// rather than calculating and sending every pixel (more traffic).
        /// </summary>
        public IList<Vector2> NewPixelsPos { get; set; } = new List<Vector2>();

        /// <summary>
        /// Last mouse position while drawing, for line interpolation. Reset to {-1,-1} on mouse up, to avoid interpolating across skips.
        /// </summary>
        private Vector2 lastPos = new Vector2(-1, -1);

        public DrawCanvas()
        {
            StyleSheet.Load("UI/DrawCanvas.scss");

            InitializeCanvasTexture();
            Style.Height = height;
            Style.Width = width;
        }

        /// <summary>
        /// Initializes a blank, white Canvas texture.
        /// </summary>
        public void InitializeCanvasTexture()
        {
            CanvasInfo = new byte[width * height * 4];
            for(int i = 0; i < CanvasInfo.Length; i++)
                CanvasInfo[i] = 255;

            Texture2DBuilder build = Texture.Create(width, height).WithDynamicUsage();
            build.WithData(CanvasInfo, CanvasInfo.Length);
            Texture = build.Finish();
        }

        /// <summary>
        /// Updates the Canvas' texture with our delta pixel data.
        /// </summary>
        public void RedrawCanvas() => Texture.Update(CanvasInfo);

        /// <summary>
        /// Fill a Pixel in on our Canvas.
        /// </summary>
        /// <param name="newPixel"></param>
        public void FillPixel(Pixel newPixel)
        {
            CanvasInfo[newPixel.Index] = newPixel.Red;
            CanvasInfo[newPixel.Index + 1] = newPixel.Green;
            CanvasInfo[newPixel.Index + 2] = newPixel.Blue;
        }

        /// <summary>
        /// (Left) Mouse button is pressed. Let's start drawing.
        /// </summary>
        protected override void OnMouseDown(MousePanelEvent e)
        {
            if(e.Button != "mouseleft") return;

            isDrawing = true;
        }

        /// <summary>
        /// (Left) Mouse button is no longer pressed. No longer drawing.
        /// </summary>
        protected override void OnMouseUp(MousePanelEvent e)
        {
            if(e.Button != "mouseleft") return;

            lastPos.x = -1; lastPos.y = -1;
            isDrawing = false;
        }


        private bool isDrawing = false;
        /// <summary>
        /// Handles drawing logic, if isDrawing == true.
        /// </summary>
        public override void Tick()
        {
            // Not trying to draw, so don't.
            if(!isDrawing) return;

            // Not current drawer, block drawing.
            if(!ClientUtil.CanDraw(Local.Client)) return;

            // Map mouse position to panel position.
            // This is still currently kinda shit and sticks out sometimes,
            // but is still good enough for me for now.
            var pos = MousePosition * ScaleFromScreen * .95;

            // Check for OOB
            if(pos.x < 0 || pos.y < 0 || pos.x > width || pos.y > height) return;

            Color32 curColor = Game.Current.CurrentColor;

            // Set lastPos early if we weren't drawing before this frame
            // (indicated by {-1, -1})
            if(lastPos.x == -1 && lastPos.y == -1)
                lastPos = pos;

            NewPixelsPos.Add(pos);
            DrawLine((int)lastPos.x, (int)lastPos.y, (int)pos.x, (int)pos.y, curColor);

            lastPos = pos;
            RedrawCanvas();
        }

        /// <summary>
        /// Finds pixels in a circular radius around pos.
        /// </summary>
        public List<int> FindPixelsInRadius(Vector2 pos, int radius)
        {
            List<int> indices = new List<int>();
            int xpos = (int)pos.x;
            int ypos = (int)pos.y;

            for(int x = xpos - radius; x <= xpos + radius; x++)
            {
                // Check for wrapping.
                if(x < 0 || x >= width) continue;

                for(int y = ypos - radius; y <= ypos + radius; y++)
                {
                    // Check for wrapping.
                    if(y < 0 || y >= height) continue;

                    // Check circular radius around pos for a nice circle drawing.
                    float distance = MathF.Sqrt((x - pos.x) * (x - pos.x) + (y - pos.y) * (y - pos.y));
                    if(distance > radius) continue;

                    // Pixel found, calculate and add index.
                    int index = (y * width * 4) + (x * 4);
                    indices.Add(index);
                }
            }

            return indices;
        }

        // Implementation of Bresenham's line algo, interpolates between points to draw w/o gaps xD
        private void DrawLine(int xStart, int yStart, int xEnd, int yEnd, Color32 color)
        {
            int xDist = Math.Abs(xEnd - xStart), xDir = xStart < xEnd ? 1 : -1;
            int yDist = Math.Abs(yEnd - yStart), yDir = yStart < yEnd ? 1 : -1;

            int err = (xDist > yDist ? xDist : -yDist) / 2, e = 0;

            for(; ; )
            {
                if(xStart >= 0 && xStart < width && yStart >= 0 && yStart < height)
                {
                    //Calculate pixels in radius locally
                    var indices = FindPixelsInRadius(new Vector2((float)xStart, (float)yStart), Game.Current.CurrentSize);
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

                if(xStart == xEnd && yStart == yEnd)
                    break;

                e = err;
                if(e > -xDist)
                {
                    err -= (int)yDist;
                    xStart += (int)xDir;
                }
                if(e < yDist)
                {
                    err += (int)xDist;
                    yStart += (int)yDir;
                }
            }
        }
    }
}
