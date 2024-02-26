using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Code.Internal.CodeRunHelpers;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class TypedCode16Helper(CodeHelperSpecs helperSpecs, IDictionary<string, object> myModelDic, object razorModel = default)
    : CodeHelperXxBase(helperSpecs, SxcLogName + ".TCd16H")
{
    public bool DefaultStrict = true;

    public object RazorModel => razorModel;

    public TModel GetModel<TModel>()
    {
        try
        {
            return (TModel)razorModel;
        }
        catch (Exception ex)
        {
            var msg = $"Failed to cast Razor Model to '{typeof(TModel)}' from '{razorModel?.GetType().Name}' - value was '{razorModel}'";
            Log.E(msg);
            throw Log.Ex(new InvalidCastException(msg, ex));
        }
    }

    internal ContextData Data { get; } = helperSpecs.CodeApiSvc.Data as ContextData;

    public ITypedItem MyItem => _myItem.Get(() => CodeRoot._Cdf.AsItem(Data.MyItem, propsRequired: DefaultStrict));
    private readonly GetOnce<ITypedItem> _myItem = new();

    public IEnumerable<ITypedItem> MyItems => _myItems.Get(() => CodeRoot._Cdf.AsItems(Data.MyItem, propsRequired: DefaultStrict));
    private readonly GetOnce<IEnumerable<ITypedItem>> _myItems = new();

    public ITypedItem MyHeader => _myHeader.Get(() => CodeRoot._Cdf.AsItem(Data.MyHeader, propsRequired: DefaultStrict));
    private readonly GetOnce<ITypedItem> _myHeader = new();

    public ITypedModel MyModel => _myModel.Get(() => new TypedModel(Specs, myModelDic, CodeRoot, Specs.IsRazor, Specs.CodeFileName));
    private readonly GetOnce<ITypedModel> _myModel = new();


    public ITypedStack AllResources => (CodeRoot as CodeApiService)?.AllResources;

    public ITypedStack AllSettings => (CodeRoot as CodeApiService)?.AllSettings;

    //public IDevTools DevTools => _devTools.Get(() => new DevTools(IsRazor, CodeFileName, Log));
    //private readonly GetOnce<IDevTools> _devTools = new GetOnce<IDevTools>();
}