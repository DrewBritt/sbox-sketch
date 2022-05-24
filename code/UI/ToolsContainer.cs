using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Sketch;

/// <summary>
/// Holds drawer's different tools (colors, radius, reset)
/// </summary>
public partial class ToolsContainer : Panel
{
    public static readonly Color[] Palette = new Color[]
    {
            Color.Red,
            Color.Orange,
            Color.Yellow,
            Color.Green,
            new Color(.1f, .51f, .04f), //Dark Green
            Color.Cyan,
            Color.Blue,
            Color.Magenta,
            Color.White,
            Color.Black
    };
    public static readonly int[] BrushSizes = new int[]
    {
            3,
            6,
            12
    };

    public Panel Colors, Sizes, TrashCan;
    public ToolsContainer()
    {
        StyleSheet.Load("ui/ToolsContainer.scss");

        Colors = Add.Panel("colors");
        foreach(Color color in Palette)
        {
            Button colorButton = new Button("", "", () => UpdateColor(color));
            colorButton.AddClass("colorbutton");
            colorButton.Style.BackgroundColor = color;
            Colors.AddChild(colorButton);
        }

        Sizes = Add.Panel("sizes");
        foreach(int size in BrushSizes)
        {
            Button sizeButton = new Button("", "", () => UpdateSize(size));
            sizeButton.AddClass("sizebutton");
            sizeButton.Style.Width = size * 5;
            sizeButton.Style.Height = size * 5;
            Sizes.AddChild(sizeButton);
        }

        TrashCan = Add.Panel("trashcan");
        TrashCan.Add.Button("", "trashbutton", () => Game.ClearCanvas());

        BindClass("enable", () => Game.Current.CurrentDrawer == Local.Client);
    }

    public void UpdateColor(Color newColor)
    {
        Game.SetCurrentColor(newColor.Rgba);
    }

    public void UpdateSize(int size)
    {
        Game.SetCurrentSize(size);
    }
}
