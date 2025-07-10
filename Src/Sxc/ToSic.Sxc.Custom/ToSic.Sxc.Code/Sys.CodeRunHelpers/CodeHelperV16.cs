using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Code.Sys.CodeRunHelpers;

/// <summary>
/// Code Helper for typed code v16+
/// </summary>
/// <param name="owner"></param>
/// <param name="helperSpecs"></param>
/// <param name="getRazorModel"></param>
/// <param name="getModelDic"></param>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class TypedCode16Helper(object owner, CompileCodeHelperSpecs helperSpecs, Func<object?> getRazorModel, Func<IDictionary<string, object>?> getModelDic)
    : CodeHelperTypedData(helperSpecs, SxcLogName + ".TCd16H")
{
    // Note: we're passing in factory methods so they don't get processed unless needed
    // Reason is that we have 2 scenarios, which can throw errors if processed in the wrong scenario
    public object? RazorModel => _razorModel.Get(getRazorModel);
    private readonly GetOnce<object?> _razorModel = new();

    public IDictionary<string, object> MyModelDic => _myModelDic.Get(getModelDic)!;
    private readonly GetOnce<IDictionary<string, object>?> _myModelDic = new();

    public TModel GetModel<TModel>()
    {
        try
        {
            return (TModel)RazorModel!;
        }
        catch (Exception ex)
        {
            var msg = $"Failed to cast Razor Model to '{typeof(TModel)}' from '{RazorModel?.GetType().Name}' - value was '{RazorModel}'";
            Log.E(msg);
            throw Log.Ex(new InvalidCastException(msg, ex));
        }
    }

    public ITypedRazorModel MyModel => _myModel.Get(() => new TypedRazorModel(Specs, MyModelDic, Specs.IsRazor, Specs.CodeFileName))!;
    private readonly GetOnce<ITypedRazorModel> _myModel = new();

    //public IDevTools DevTools => _devTools.Get(() => new DevTools(IsRazor, CodeFileName, Log));
    //private readonly GetOnce<IDevTools> _devTools = new GetOnce<IDevTools>();

    public TService GetService<TService>(NoParamOrder protector = default, string? typeName = default) where TService : class
    {
        if (typeName.IsEmptyOrWs())
            return ExCtx.GetService<TService>();

        var ownType = owner.GetType();
        var assembly = ownType.Assembly;
        // Note: don't check the Namespace property, as it may be empty
        if (!HotBuildConstants.ObjectIsFromAppCode(owner))
            throw Log.Ex(new Exception($"Type '{ownType.FullName}' is not in the 'AppCode' namespace / dll, so it can't be used to find other types."));

        var type = assembly.FindControllerTypeByName(typeName);
        
        return type == null
            ? throw Log.Ex(new Exception($"Type '{typeName}' not found in assembly '{assembly.FullName}'"))
            : ExCtx.GetService<TService>(type: type);
    }


}