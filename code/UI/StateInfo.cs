using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Sketch
{
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
            StateTime = StateContainer.Add.Label("00:00", "statetime");

            WordContainer = Add.Panel("wordcontainer");
            Letters = WordContainer.Add.Label("");
        }

        public override void Tick()
        {
            var game = Game.Current;
            if(game == null) return;

            StateName.Text = game.CurrentStateName.ToUpper();
            StateTime.Text = game.CurrentStateTime;

            //Hide container if no letters to display
            if(Letters.Text == "")
            {
                if(!WordContainer.HasClass("empty"))
                    WordContainer.SetClass("empty", true);
                return;
            }

            //Otherwise display
            if(WordContainer.HasClass("empty"))
                WordContainer.RemoveClass("empty");
        }
    }
}
