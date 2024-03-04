// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class RazorTyped<TModel> : RazorTyped
{
    public TModel Model => CodeHelper.GetModel<TModel>();
}