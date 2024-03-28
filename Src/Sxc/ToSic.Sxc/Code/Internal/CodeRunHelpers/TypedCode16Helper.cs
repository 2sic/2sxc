using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Code.Internal.CodeRunHelpers;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class TypedCode16Helper(CodeHelperSpecs helperSpecs, Func<object> getRazorModel, Func<IDictionary<string, object>> getModelDic)
    : CodeHelperXxBase(helperSpecs, SxcLogName + ".TCd16H")
{
    public bool DefaultStrict = true;

    // Note: we're passing in factory methods so they don't get processed unless needed
    // Reason is that we have 2 scenarios, which can throw errors if processed in the wrong scenario
    public object RazorModel => _razorModel.Get(getRazorModel);
    private readonly GetOnce<object> _razorModel = new();

    public IDictionary<string, object> MyModelDic => _myModelDic.Get(getModelDic);
    private readonly GetOnce<IDictionary<string, object>> _myModelDic = new();

    public TModel GetModel<TModel>()
    {
        try
        {
            return (TModel)RazorModel;
        }
        catch (Exception ex)
        {
            var msg = $"Failed to cast Razor Model to '{typeof(TModel)}' from '{RazorModel?.GetType().Name}' - value was '{RazorModel}'";
            Log.E(msg);
            throw Log.Ex(new InvalidCastException(msg, ex));
        }
    }

    internal ContextData Data { get; } = helperSpecs.CodeApiSvc.Data as ContextData;

    public ITypedItem MyItem => _myItem.Get(() => CodeRoot.Cdf.AsItem(Data.MyItem, propsRequired: DefaultStrict));
    private readonly GetOnce<ITypedItem> _myItem = new();

    public IEnumerable<ITypedItem> MyItems => _myItems.Get(() => CodeRoot.Cdf.AsItems(Data.MyItem, propsRequired: DefaultStrict));
    private readonly GetOnce<IEnumerable<ITypedItem>> _myItems = new();

    public ITypedItem MyHeader => _myHeader.Get(() => CodeRoot.Cdf.AsItem(Data.MyHeader, propsRequired: DefaultStrict));
    private readonly GetOnce<ITypedItem> _myHeader = new();

    public ITypedModel MyModel => _myModel.Get(() => new TypedModel(Specs, MyModelDic, Specs.IsRazor, Specs.CodeFileName));
    private readonly GetOnce<ITypedModel> _myModel = new();


    public ITypedStack AllResources => (CodeRoot as CodeApiService)?.AllResources;

    public ITypedStack AllSettings => (CodeRoot as CodeApiService)?.AllSettings;

    //public IDevTools DevTools => _devTools.Get(() => new DevTools(IsRazor, CodeFileName, Log));
    //private readonly GetOnce<IDevTools> _devTools = new GetOnce<IDevTools>();
}