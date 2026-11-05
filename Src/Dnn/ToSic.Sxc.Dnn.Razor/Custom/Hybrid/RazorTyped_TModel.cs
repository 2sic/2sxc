using ToSic.Sxc.Code.Sys.CodeRunHelpers;
using ToSic.Sxc.Render.Engines.Sys;

#pragma warning disable IDE0130
namespace Custom.Hybrid;
#pragma warning restore IDE0130

/// <summary>
/// New base class with custom model.
/// The rest of this is identical to <see cref="RazorTyped"/>.
/// </summary>
/// <typeparam name="TModel">Model type - like `string` or a class from your `AppCode`</typeparam>
/// <remarks>
/// Introduced in v17.03
/// </remarks>
[PublicApi]
public abstract class RazorTyped<TModel> : RazorTyped, ISetDynamicModel
{
    /// <summary>
    /// The model for this Razor file.
    /// Typed according to the `@inherits` statement.
    /// </summary>
    public TModel Model => RazorModelHlp.GetModel<TModel>();

    internal TypedRazorModelHelper RazorModelHlp => field ??= CreateRazorModelHlp();


    private TypedRazorModelHelper CreateRazorModelHlp() =>
        new(parentLog: CodeHelper.Log,
            getRazorModel: () => OverridePageData
                                 // the default/only value would be on a 0 key
                                 ?? (PageData?.TryGetValue(0, out var zeroData) ?? false ? zeroData as object : null)
        );


}