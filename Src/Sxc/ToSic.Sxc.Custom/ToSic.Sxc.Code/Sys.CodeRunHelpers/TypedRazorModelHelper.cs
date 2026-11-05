using ToSic.Sys;

namespace ToSic.Sxc.Code.Sys.CodeRunHelpers;

/// <summary>
/// Code Helper for handling typed razor models.
/// </summary>
/// <param name="parentLog">The parent log to use for logging</param>
/// <param name="getRazorModel">A function to get the razor model</param>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class TypedRazorModelHelper(Func<object?> getRazorModel, ILog? parentLog) : HelperBase(parentLog, $"{SxcLogName}.TdC16H")
{

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

    private object? RazorModel => _razorModel.Get(getRazorModel);
    private readonly LazyGet<object?> _razorModel = new();


    /// <summary>
    /// Helper to create the model, as the underlying objects are internal.
    /// </summary>
    /// <param name="helperSpecs"></param>
    /// <param name="getModelDic"></param>
    /// <returns></returns>
    public static ITypedRazorModel GetMyModel(CompileCodeHelperSpecs helperSpecs, IDictionary<string, object?>? getModelDic)
        => new TypedRazorModel(helperSpecs, getModelDic);
}
