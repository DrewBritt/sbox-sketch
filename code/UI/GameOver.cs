using System.Linq;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Sketch;

/// <summary>
/// Popup at end of game that displays top 3 as well as a general scoreboard
/// </summary>
public partial class GameOver : Panel
{
    public Panel Top3Container { get; set; }

    public GameOver()
    {
        StyleSheet.Load("/ui/GameOver.scss");
        Add.Label("GAME OVER", "title");

        Top3Container = Add.Panel("top3container");
        Top3Init();

        var scoreboard = Add.Panel("scoreboard");
        scoreboard.AddChild<Scoreboard>();
    }

    public void Top3Init()
    {
        //Sorted 1st 2nd 3rd (we need 2nd 1st 3rd)
        Client[] top3 = Client.All.OrderByDescending(c => c.GetInt("GameScore")).Take(3).ToArray();

        //Swapsies
        if(top3.Length > 1)
        {
            var temp = top3[0];
            top3[0] = top3[1];
            top3[1] = temp;
        }

        //Add top 3 panels
        for(int i = 0; i < top3.Length; i++)
        {
            Client c = top3[i];
            var p = Top3Container.Add.Panel("entry");
            if(i == 1)
                p.AddClass("first");

            p.Add.Image($"avatarbig:{c.PlayerId}");
            p.Add.Label(c.Name);
            p.Add.Label(c.GetInt("GameScore").ToString());
        }
    }
}
