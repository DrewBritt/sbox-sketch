using Sandbox.UI;


namespace Sketch
{
	public partial class DrawCanvas : Panel
	{
		public Panel Container { get; set; }
		public DrawCanvas()
		{
			StyleSheet.Load( "UI/DrawCanvas.scss" );

			Container = Add.Panel( "container" );
		}
	}
}
