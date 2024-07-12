using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Code.Internal.CodeRunHelpers;

/// <summary>
/// Code Helper for typed code v16+
/// </summary>
/// <param name="owner"></param>
/// <param name="helperSpecs"></param>
/// <param name="getRazorModel"></param>
/// <param name="getModelDic"></param>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class TypedCode16Helper(object owner, CodeHelperSpecs helperSpecs, Func<object> getRazorModel, Func<IDictionary<string, object>> getModelDic)
    : CodeHelperV00Base(helperSpecs, SxcLogName + ".TCd16H")
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

    public ITypedItem MyItem => _myItem.Get(() => CodeApiSvc.Cdf.AsItem(Data.MyItem.FirstOrDefault(), propsRequired: DefaultStrict));
    private readonly GetOnce<ITypedItem> _myItem = new();

    public IEnumerable<ITypedItem> MyItems => _myItems.Get(() => CodeApiSvc.Cdf.EntitiesToItems(Data.MyItem, propsRequired: DefaultStrict));
    private readonly GetOnce<IEnumerable<ITypedItem>> _myItems = new();

    public ITypedItem MyHeader => _myHeader.Get(() => CodeApiSvc.Cdf.AsItem(Data.MyHeader.FirstOrDefault(), propsRequired: DefaultStrict));
    private readonly GetOnce<ITypedItem> _myHeader = new();

    public ITypedModel MyModel => _myModel.Get(() => new TypedModel(Specs, MyModelDic, Specs.IsRazor, Specs.CodeFileName));
    private readonly GetOnce<ITypedModel> _myModel = new();


    public ITypedStack AllResources => (CodeApiSvc as CodeApiService)?.AllResources;

    public ITypedStack AllSettings => (CodeApiSvc as CodeApiService)?.AllSettings;

    //public IDevTools DevTools => _devTools.Get(() => new DevTools(IsRazor, CodeFileName, Log));
    //private readonly GetOnce<IDevTools> _devTools = new GetOnce<IDevTools>();

    public TService GetService<TService>(NoParamOrder protector = default, string typeName = default) where TService : class
    {
        if (typeName.IsEmptyOrWs())
            return CodeApiSvc.GetService<TService>();

        var ownType = owner.GetType();
        var assembly = ownType.Assembly;
        // Note: don't check the Namespace property, as it may be empty
        if (!HotBuildConstants.ObjectIsFromAppCode(owner))
            throw Log.Ex(new Exception($"Type '{ownType.FullName}' is not in the 'AppCode' namespace / dll, so it can't be used to find other types."));

        var type = assembly.FindControllerTypeByName(typeName);
        
        return type == null
            ? throw Log.Ex(new Exception($"Type '{typeName}' not found in assembly '{assembly.FullName}'"))
            : CodeApiSvc.GetService<TService>(type: type);
    }


}