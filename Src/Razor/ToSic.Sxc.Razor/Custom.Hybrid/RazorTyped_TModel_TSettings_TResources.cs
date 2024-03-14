// 2024-03-14 2dm - experimental, don't use yet
// will probably never make it, as we prefer to use Customize on each AppRazor
// since we would also need parameters for View<TSettings, TResources> etc.


//using ToSic.Lib.Documentation;
//using ToSic.Sxc.Apps;
//using ToSic.Sxc.Data;

//// ReSharper disable once CheckNamespace
//namespace Custom.Hybrid;

//[PrivateApi]
//[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
//public abstract class RazorTyped<TModel, TSettings, TResources> : RazorTyped<TModel>
//  where TSettings : class, ITypedItem, ITypedItemWrapper16, new()
//  where TResources : class, ITypedItem, ITypedItemWrapper16, new()
//{
//  public new IAppTyped<TSettings, TResources> App => _app ??= Customize.App<TSettings, TResources>(); // GetService<IAppTyped<TSettings, TResources>>(reuse: true);
//  private IAppTyped<TSettings, TResources> _app;
//}