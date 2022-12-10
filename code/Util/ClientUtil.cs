using Sandbox;

namespace Sketch;
public static class ClientUtil
{
    /// <summary>
    /// Returns whether or not the passed client is able to draw.
    /// </summary>
    /// <returns></returns>
    public static bool CanDraw(IClient cl)
    {
        if(GameManager.Current.CurrentDrawer != cl)
            return false;

        if(GameManager.Current.CurrentStateName != "Playing")
            return false;

        return true;
    }
}
