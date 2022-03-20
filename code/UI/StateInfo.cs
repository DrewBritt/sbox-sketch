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

        Sound countdownSound, endSound;
        public override void Tick()
        {
            var game = Game.Current;
            if (game == null) return;

            StateName.Text = game.CurrentStateName.ToUpper();
            StateTime.Text = game.CurrentStateTime;

            if(game.CurrentStateName == "Playing")
            {
                //3-2-1 countdown
                if((game.CurrentStateTime == "00:03" || game.CurrentStateTime == "00:02" || game.CurrentStateTime == "00:01") && countdownSound.Finished)
                    countdownSound = Sound.FromScreen("pingshort");

                if (game.CurrentStateTime == "00:00" && endSound.Finished)
                    endSound = Sound.FromScreen("belllong");
            }

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
