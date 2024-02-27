using ToSic.Sxc.Data;
using ToSic.Sxc.Apps;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class RazorTyped<TModel, TSettings, TResources> : RazorTyped
    where TSettings : class, ITypedItem, ITypedItemWrapper16, new()
    where TResources : class, ITypedItem, ITypedItemWrapper16, new()
{
    public TModel Model => CodeHelper.GetModel<TModel>();

    public new IAppTyped<TSettings, TResources> App => _app.Get(GetService<IAppTyped<TSettings, TResources>>);
    private readonly GetOnce<IAppTyped<TSettings, TResources>> _app = new();
}