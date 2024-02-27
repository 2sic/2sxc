using ToSic.Sxc.Data;
using ToSic.Sxc.Apps;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class RazorTyped<TModel, TSettings, TResources> : RazorTyped<TModel>
    where TSettings : class, ITypedItem, ITypedItemWrapper16, new()
    where TResources : class, ITypedItem, ITypedItemWrapper16, new()
{
    public new IAppTyped<TSettings, TResources> App => _app ??= GetService<IAppTyped<TSettings, TResources>>(reuse: true);
    private IAppTyped<TSettings, TResources> _app;
}