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
    public static ToolbarBuilder AddInternal(this ToolbarBuilder original, object[] newRules, [CallerMemberName] string? cName = default)
    {
        var l = original.Log.Fn<ToolbarBuilder>(cName);
        if (!newRules.Any())
            return l.Return(original, "no new rules");

        //var mergeRules = new List<ToolbarRuleBase>(original.Rules);
        //foreach (var rule in newRules)
        //    switch (rule)
        //    {
        //        case ToolbarRuleBase realRule:
        //            mergeRules.Add(realRule);
        //            break;
        //        case string stringRule:
        //            mergeRules.Add(new ToolbarRuleGeneric(stringRule));
        //            break;
        //    }

        var typedNewRules = newRules
            .Select(rule => rule switch
            {
                ToolbarRuleBase realRule => realRule,
                string stringRule => new ToolbarRuleGeneric(stringRule),
                _ => null!
            })
            .Where(r => r != null)
            .ToList();

        // Create clone with all rules
        var clone = original with
        {
            Rules = original.Rules.Concat(typedNewRules).ToList(),
        };


        return l.Return(clone, $"clone with {typedNewRules.Count} new rules");
    }

}