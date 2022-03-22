using System.Collections.Generic;
using System.Linq;
using Sandbox;
using static Sketch.Game;

namespace Sketch
{
    public static class ClientUtil
    {
        /// <summary>
        /// Returns a list of all clients except for the drawer, stored at drawerindex.
        /// Maybe find a not stupid way to remove from IReadOnlyList?
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> ClientsExceptDrawer(IReadOnlyList<Client> all, Client drawer)
        {
            List<Client> drawerlist = new List<Client>();
            drawerlist.Add(drawer);
            return all.Except(drawerlist);
        }

        /// <summary>
        /// Returns whether or not the passed client is able to draw.
        /// </summary>
        /// <returns></returns>
        public static bool CanDraw(Client cl)
        {
            if(Current.CurrentDrawer != cl)
                return false;

            if(Current.CurrentStateName != "Playing")
                return false;

            return true;
        }
    }
}
