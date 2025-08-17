using ToSic.Razor.Blade;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Sys.Render.PageContext;
using ToSic.Sxc.Sys.Render.PageFeatures;

namespace ToSic.Sxc.Services.Page.Sys;

public class PageChangeListenerManagerWip
{
    public List<RenderResult> RenderListeners = [];

    public RenderResult CreateRenderListener()
    {
        var listener = new RenderResult
        {
            Features = [],
            FeaturesFromResources = [],
            PartialActivateWip = [],
            PartialModuleTags = [],
            HeadChanges = [],
        };
        RenderListeners.Add(listener);
        return listener;
    }

    public void RemoveListener(RenderResult listener)
        => RenderListeners.Remove(listener);

    public void Activate(string[] keys)
    {
        foreach (var renderListener in RenderListeners)
            renderListener.PartialActivateWip!.AddRange(keys);
    }

    public void AddResource(PageFeatureFromSettings feature)
    {
        foreach (var renderListener in RenderListeners)
            renderListener.FeaturesFromResources!.Add(feature with { }); // clone to avoid modifying the original, which is cached
    }

    public void AddPartialModuleTag(IHtmlTag tag, bool noDuplicates)
    {
        foreach (var renderListener in RenderListeners)
            renderListener.PartialModuleTags!.Add((tag, noDuplicates));
    }

    public void AddToHead(HeadChange headChange)
    {
        foreach (var renderListener in RenderListeners)
            renderListener.HeadChanges!.Add(headChange);
    }
}