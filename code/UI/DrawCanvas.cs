using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace Sketch
{
	public partial class DrawCanvas : Image
	{
		int width = 800;
		int height = 600;

		/// <summary>
		/// RGBA data for Canvas texture.
		/// </summary>
		public byte[] CanvasInfo;

		public List<int> updatedPixels { get; set; } = new List<int>();
		public DrawCanvas()
		{
			StyleSheet.Load( "UI/DrawCanvas.scss" );

			InitializeCanvasTexture();
			Style.Height = height;
			Style.Width = width;
		}

		public void InitializeCanvasTexture()
		{
			CanvasInfo = new byte[width * height * 4];
			for ( int i = 0; i < CanvasInfo.Length; i++ )
				CanvasInfo[i] = 255;

			Texture2DBuilder build = Texture.Create( width, height );
			build.WithData(CanvasInfo, CanvasInfo.Length);
			Texture = build.Finish();
		}

		public void RedrawCanvas() => Texture.Update( CanvasInfo );

		public void UpdateCanvasInfo(int index, int r, int g, int b)
		{
			CanvasInfo[index] = (byte)r;
			CanvasInfo[index + 1] = (byte)g;
			CanvasInfo[index + 2] = (byte)b;


			if ( Client.All[Game.Current.CurrentDrawerIndex] == Local.Client )
			{
				updatedPixels.Add( index );
				updatedPixels.Add( r );
				updatedPixels.Add( g );
				updatedPixels.Add( b );
			}
		}

		//Left click down + dragging mouse
		//TODO: Proper input probably? Too much of a dumbfuck to figure out
		//how to check Attack1, UI is taking all the mouse events.
		protected override void OnDragSelect( SelectionEvent e )
		{
			var pos = ScreenPositionToPanelPosition( e.EndPoint);
			if (pos.x < 0 || pos.y < 0 || pos.x > width || pos.y > height)
				return;
			Log.Info($"MousePos: {Mouse.Position}, MousePosToPanelPos: {pos}");

			var indexes = FindPixelsInDistance( pos, 1 );
			foreach ( var index in indexes )
				UpdateCanvasInfo( index, 255, 0, 0 );
			RedrawCanvas();
		}

		public List<int> FindPixelsInDistance(Vector2 pos, int radius)
		{
			List<int> indexes = new List<int>();
			int xpos = (int)pos.x;
			int ypos = (int)pos.y;

			for(int x = xpos - radius; x <= xpos + radius; x++ )
			{
				//Check for wrapping
				if ( x < 0 || x >= width )
					continue;

				for(int y = ypos - radius; y <= ypos + radius; y++ )
				{
					//Check for wrapping
					if ( y < 0 || y >= height )
						continue;

					//Pixel found, calculate and add index
					int index = (y * width * 4) + (x*4);
					indexes.Add(index);
				}
			}

			return indexes;
		}
	}
}
