using ToSic.Sxc.Web.Internal.Url;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Edit.Toolbar.Internal;

/// <summary>
/// Special utilities for the Toolbar Builder,
/// which should only be created once per use case to optimize performance.
///
/// Note that previously this was part of the ToolbarBuilder,
/// but the effect was since the toolbar builder is re-created constantly, the
/// helpers were also recreated constantly. 
/// </summary>
internal class ToolbarBuilderUtilities
{
    #region Parameters Processing

    /// <summary>
    /// Helper to process 'parameters' to url, ensuring lower-case etc. 
    /// </summary>
    [field: AllowNull, MaybeNull]
    public ObjectToUrl Par2Url => field ??= new(null, [new UrlValueCamelCase()]);


    /// <summary>
    /// Helper to process 'filter' to url - should not change the case of the properties and auto-fix some special scenarios
    /// </summary>
    [field: AllowNull, MaybeNull]
    public ObjectToUrl Filter2Url => field ??= new(null, [new FilterValueProcessor()])
    {
        ArrayBoxStart = "[",
        ArrayBoxEnd = "]"
    };



    /// <summary>
    /// Helper to process 'prefill' - should not change the case of the properties
    /// </summary>
    [field: AllowNull, MaybeNull]
    public ObjectToUrl Prefill2Url => field ??= new(null);

    public string? PrepareParams(object? parameters, ITweakButton? tweaks = null)
    {
        var strParams = Par2Url.Serialize(parameters);
        return MergeWithTweaks(strParams, (tweaks as ITweakButtonInternal)?.ParamsMerge);
    }

    #endregion


    #region UI Processing

    internal static ObjectToUrl GetUi2Url() => new(null, [
        new UrlValueCamelCase(),
        new UiValueProcessor()
    ]);


    public string? PrepareUi(
        object? ui,
        object? uiMerge = null,
        string? uiMergePrefix = null,
        string? group = null, // current button-group name which must be merged into the Ui parameter
        IEnumerable<object?>? tweaks = default
    )
    {
        var uiString = Ui2Url.SerializeWithChild(ui, uiMerge, uiMergePrefix);
        if (group.HasValue())
            uiString = Ui2Url.SerializeWithChild(uiString, $"group={group}");
        return MergeWithTweaks(uiString, tweaks);
    }

    [field: AllowNull, MaybeNull]
    private ObjectToUrl Ui2Url => field ??= GetUi2Url();

    #endregion

    /// <summary>
    ///  new v15 - add UI tweaks - must come last / after group
    /// </summary>
    /// <param name="previous"></param>
    /// <param name="tweaks"></param>
    /// <returns></returns>
    private string? MergeWithTweaks(string? previous, IEnumerable<object?>? tweaks = null)
    {
        // new v15 - add UI tweaks - must come last / after group
        if (tweaks != null) 
            previous = tweaks.Aggregate(previous, (prev, t) => Ui2Url.SerializeWithChild(prev, t));
        return previous;

    }
}