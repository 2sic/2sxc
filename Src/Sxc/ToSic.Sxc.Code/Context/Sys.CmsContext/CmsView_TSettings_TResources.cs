using ToSic.Eav.Models;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Context.Sys.CmsContext;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TSettings"></typeparam>
/// <typeparam name="TResources"></typeparam>
/// <param name="parent"></param>
/// <param name="block"></param>
/// <param name="settingsPropsRequired">
/// Problem: in v16/17 we forgot to make propsRequired for Setting be true.
/// So they were `false` - and the Content App used such settings which didn't exist! :(
/// So for now we need to keep this as `true` - but we should change it back to `false` in
/// the next base class update, probably v18.
/// </param>
internal class CmsView<TSettings, TResources>(CmsContext parent, IBlock block, bool settingsPropsRequired = true)
    : CmsView(parent, block), ICmsView<TSettings, TResources>
    where TSettings : class, IModelOfData, new()
    where TResources : class, IModelOfData, new()
{
    private readonly IView _view = block.View!;

    [field: AllowNull, MaybeNull]
    private ICodeDataFactory Cdf => field ??= Parent.ExCtx.GetCdf();

    public TSettings? Settings => _settings.Get(() => Cdf.AsCustom<TSettings>(Cdf.AsItem(_view.Settings, new() { ItemIsStrict = settingsPropsRequired })));
    private readonly GetOnce<TSettings?> _settings = new();

    public TResources? Resources => _resources.Get(() => Cdf.AsCustom<TResources>(_view.Resources));
    private readonly GetOnce<TResources?> _resources = new();
}