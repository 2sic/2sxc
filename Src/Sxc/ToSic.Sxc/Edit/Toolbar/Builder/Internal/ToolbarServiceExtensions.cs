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
    public static IToolbarBuilder AddInternal(this IToolbarBuilder original, params object[] newRules)
    {
        var l = original.Log.Fn<IToolbarBuilder>();
        if (newRules == null || !newRules.Any())
            return l.Return(original, "no new rules");

        // Create clone before starting to log so it's in there too
        var clone = new ToolbarBuilder(parent: original as ToolbarBuilder);

        foreach (var rule in newRules)
            if (rule is ToolbarRuleBase realRule)
                clone.Rules.Add(realRule);
            else if (rule is string stringRule)
                clone.Rules.Add(new ToolbarRuleGeneric(stringRule));

        return l.Return(clone, "clone");
    }

}