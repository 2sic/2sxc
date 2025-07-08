using System.Runtime.CompilerServices;
using ToSic.Sxc.Edit.Toolbar.Sys.Rules;

namespace ToSic.Sxc.Edit.Toolbar.Sys.ToolbarBuilder;

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
    public static ToolbarBuilder AddInternal(this ToolbarBuilder original, ToolbarRuleBase[] newRules, [CallerMemberName] string? methodName = default)
    {
        var l = original.Log.Fn<ToolbarBuilder>(methodName);
        if (!newRules.Any())
            return l.Return(original, "no new rules");

        // Create clone with all rules
        var clone = original with
        {
            Rules = original.Rules.Concat(newRules).ToList(),
        };


        return l.Return(clone, $"clone with {newRules.Length} new rules");
    }

}