using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Sketch
{
    public static class ClientUtil
    {
        /// <summary>
        /// Returns a list of all clients except for the drawer, stored at drawerindex.
        /// Maybe find a not stupid way to remove from IReadOnlyList?
        /// </summary>
        /// <param name="all"></param>
        /// <param name="drawerindex"></param>
        /// <returns></returns>
        public static IEnumerable<Client> ClientsExceptDrawer(IReadOnlyList<Client> all, int drawerindex)
        {
            List<Client> drawerlist = new List<Client>();
            drawerlist.Add(all[drawerindex]);
            return all.Except(drawerlist);
        }
    }
}
