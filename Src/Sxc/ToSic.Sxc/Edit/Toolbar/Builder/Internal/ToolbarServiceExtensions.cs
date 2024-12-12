namespace ToSic.Sxc.Edit.Toolbar.Internal;

internal static class ToolbarBuilderExtensions
{
    /// <summary>
    /// Add one or more rules (as strings or ToolbarRule objects) according to the conventions of the [js toolbar](xref:JsCode.Toolbars.Simple)
    /// </summary>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    /// <remarks>
    /// History
    /// * Added in 2sxc 13
    /// </remarks>
    public static ToolbarBuilder AddInternal(this ToolbarBuilder original, object[] newRules)
    {
        var l = original.Log.Fn<ToolbarBuilder>();
        if (newRules == null || !newRules.Any())
            return l.Return(original, "no new rules");

        var mergeRules = new List<ToolbarRuleBase>(original.Rules);
        foreach (var rule in newRules)
            switch (rule)
            {
                case ToolbarRuleBase realRule:
                    mergeRules.Add(realRule);
                    break;
                case string stringRule:
                    mergeRules.Add(new ToolbarRuleGeneric(stringRule));
                    break;
            }

        // Create clone with all rules
        var clone = original with
        {
            Rules = mergeRules
        };


        return l.Return(clone, "clone");
    }

}