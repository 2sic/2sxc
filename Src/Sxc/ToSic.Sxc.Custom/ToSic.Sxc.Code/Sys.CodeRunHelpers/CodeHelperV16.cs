namespace ToSic.Sxc.Code.Sys.CodeRunHelpers;

/// <summary>
/// Code Helper for typed code v16+
/// </summary>
/// <param name="helperSpecs"></param>
/// <param name="getRazorModel"></param>
/// <param name="getModelDic"></param>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class TypedCode16Helper(CompileCodeHelperSpecs helperSpecs, Func<object?> getRazorModel, Func<IDictionary<string, object>?> getModelDic)
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

}