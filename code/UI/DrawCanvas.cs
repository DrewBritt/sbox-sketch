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

		}
	}
}
