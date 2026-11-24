using ToSic.Eav.Apps.Views.Sys;
using ToSic.Eav.Models;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Context.Sys.CmsContext;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TSettings"></typeparam>
/// <typeparam name="TResources"></typeparam>
/// <param name="cmsContext"></param>
/// <param name="block"></param>
/// <param name="settingsPropsRequired">
/// Problem: in v16/17 we forgot to make propsRequired for Setting be true.
/// So they were `false` - and the Content App used such settings which didn't exist! :(
/// So for now we need to keep this as `true` - but we should change it back to `false` in
/// the next base class update, probably v18.
/// </param>
internal class CmsView<TSettings, TResources>(CmsContext cmsContext, IBlock block, bool settingsPropsRequired = true)
    : CmsView(cmsContext, block), ICmsView<TSettings, TResources>
    where TSettings : class, IModelFromData, new()
    where TResources : class, IModelFromData, new()
{
    private readonly IView _view = block.View!;


    public TSettings? Settings => _settings.Get(()
        => CmsContext.Cdf.AsCustom<TSettings>(CmsContext.Cdf.AsItem(_view.Settings, new() { ItemIsStrict = settingsPropsRequired })));
    private readonly LazyGet<TSettings?> _settings = new();

    public TResources? Resources => _resources.Get(()
        => CmsContext.Cdf.AsCustom<TResources>(_view.Resources));
    private readonly LazyGet<TResources?> _resources = new();
}