using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Sketch;

/// <summary>
/// Holds state information and current word information
/// </summary>
public partial class StateInfo : Panel
{
    public Panel StateContainer, WordContainer;
    public Label RoundInfo, StateName, StateTime;
    public Label Letters;

    public string RoundInfoString => $"ROUND {Game.Current.CurRound}/{Game.MaxRounds}";
    public string CurrentStateName => Game.Current.CurrentStateName.ToUpper();
    public string CurrentStateTime => Game.Current.CurrentStateTime;

    public StateInfo()
    {
        StyleSheet.Load("/ui/StateInfo.scss");

        StateContainer = Add.Panel("statecontainer");

        RoundInfo = StateContainer.Add.Label("Round x/x", "roundinfo");
        RoundInfo.Bind("text", this, "RoundInfoString");

        StateName = StateContainer.Add.Label("State", "statename");
        StateName.Bind("text", this, "CurrentStateName");

        StateTime = StateContainer.Add.Label("00:00", "statetime");
        StateTime.Bind("text", this, "CurrentStateTime");

        WordContainer = Add.Panel("wordcontainer");
        Letters = WordContainer.Add.Label("");
        WordContainer.BindClass("empty", () => Letters.Text == "");
    }
}
