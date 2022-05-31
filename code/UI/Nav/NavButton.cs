using System.Linq;
using Sandbox;
using Sandbox.UI;

namespace Sketch;

[Library("navbutton")]
public class NavButton : Button
{
    NavPanel Nav;
    public string HRef { get; set; }

    public NavButton()
    {
        Nav = Ancestors.OfType<NavPanel>().FirstOrDefault();
        BindClass("active", () => Nav.ContentURL == HRef);
    }

    public override void SetProperty(string name, string value)
    {
        base.SetProperty(name, value);

        if(name == "href")
        {
            HRef = value;
        }
    }

    protected override void OnMouseDown(MousePanelEvent e)
    {
        if(e.Button == "mouseleft")
        {
            CreateEvent("navigate", HRef);
            e.StopPropagation();
        }
    }
}
