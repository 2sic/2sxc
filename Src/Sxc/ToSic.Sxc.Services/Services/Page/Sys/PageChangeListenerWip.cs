using ToSic.Sxc.Sys.Render.PageFeatures;

namespace ToSic.Sxc.Services.Page.Sys;
public class PageChangeListenerWip
{
    public List<string> Activate{ get; } = [];

    public List<IPageFeature> PageFeatures { get; } = [];

}

public class PageChangeListenerManagerWip
{
    public List<PageChangeListenerWip> Listeners = [];

    public PageChangeListenerWip CreateListener()
    {
        var listener = new PageChangeListenerWip();
        Listeners.Add(listener);
        return listener;
    }

    public void AddListener(PageChangeListenerWip listener)
    {
        Listeners.Add(listener);
    }

    public void RemoveListener(PageChangeListenerWip listener)
    {
        Listeners.Remove(listener);
    }

    public void Activate(string[] keys)
    {
        foreach (var listener in Listeners)
            listener.Activate.AddRange(keys);
    }

    public void AddPageFeature(IPageFeature feature)
    {
        foreach (var listener in Listeners)
            listener.PageFeatures.Add(feature);
    }
}