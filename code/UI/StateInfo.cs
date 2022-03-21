using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Sketch
{
    public partial class StateInfo : Panel
    {
        public Panel Container, InfoContainer;
        public Label StateName, StateTime;

        public Label BlankLetters;
        private bool setLettersClass = false;

        public StateInfo()
        {
            StyleSheet.Load("/ui/StateInfo.scss");

            Container = Add.Panel("container");
            InfoContainer = Container.Add.Panel("infocontainer");
            StateName = InfoContainer.Add.Label("State", "statename");
            StateTime = InfoContainer.Add.Label("00:00", "statetime");
            BlankLetters = Container.Add.Label("");
        }

        public override void Tick()
        {
            var game = Game.Current;
            if (game == null) return;

            StateName.Text = game.CurrentStateName.ToUpper();
            StateTime.Text = game.CurrentStateTime;

            //TODO: Make this less shit
            if (BlankLetters.Text == "")
            {
                if (setLettersClass)
                {
                    BlankLetters.RemoveClass("blankletters");
                    setLettersClass = false;
                }

                return;
            }

            if (!setLettersClass)
            {
                BlankLetters.AddClass("blankletters");
                setLettersClass = true;
            }
        }
    }
}
