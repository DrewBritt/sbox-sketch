using System.Collections.Generic;
using System.Linq;
using Sandbox.UI;

namespace Sketch.UI;

public partial class NavPanel : Panel
{
    public Panel NavContent { get; set; }
    public string ContentURL { get; private set; }
    private List<HistoryItem> Cache { get; set; } = new List<HistoryItem>();

    protected class HistoryItem
    {
        public string Url;
        public Panel Panel;
    }

    public void Navigate(string url)
    {
        var attribute = NavTargetAttribute.Get(url);
        if(attribute is null)
        {
            Log.Error($"Sketch: NavTarget not found: {url}");
            return;
        }

        HistoryItem cached = Cache.Where(hi => hi.Url == url).FirstOrDefault();
        Panel toLoad;
        if(cached != null)
            toLoad = cached.Panel;
        else
        {
            toLoad = TypeLibrary.Create<Panel>(attribute.TargetType);
            HistoryItem hi = new HistoryItem()
            {
                Url = url,
                Panel = toLoad
            };
            Cache.Add(hi);
            Log.Info(Cache);
        }

        if(NavContent.HasChildren)
            NavContent.GetChild(0).Parent = null;
        toLoad.Parent = NavContent;
        ContentURL = url;
    }

    /// <summary>
    /// Navigates to the given URL
    /// </summary>
    /// <param name="url">URL of content to load</param>
    [PanelEvent]
    public bool NavigateEvent(string url)
    {
        Navigate(url);
        return false;
    }
}
