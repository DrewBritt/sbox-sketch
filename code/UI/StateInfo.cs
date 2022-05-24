using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Sketch;

/// <summary>
/// Holds state information and current word information
/// </summary>
public partial class StateInfo : Panel
{
    public Panel StateContainer, WordContainer;
    public Label StateName, StateTime;
    public Label Letters;

    public StateInfo()
    {
        StyleSheet.Load("/ui/StateInfo.scss");

        StateContainer = Add.Panel("statecontainer");

        StateName = StateContainer.Add.Label("State", "statename");
        StateName.Bind("text", () => Game.Current.CurrentStateName.ToUpper());

        StateTime = StateContainer.Add.Label("00:00", "statetime");
        StateTime.Bind("text", () => Game.Current.CurrentStateTime);

        WordContainer = Add.Panel("wordcontainer");
        Letters = WordContainer.Add.Label("");
        WordContainer.BindClass("empty", () => Letters.Text == "");
    }
}
