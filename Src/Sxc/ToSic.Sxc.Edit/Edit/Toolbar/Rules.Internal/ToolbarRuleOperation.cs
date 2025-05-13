using ToSic.Eav.Plumbing;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar;

internal enum ToolbarRuleOps
{
    OprAdd = ToolbarRuleOperation.AddOperation,
    OprAuto = ToolbarRuleOperation.AutoOperation,
    OprModify = ToolbarRuleOperation.ModifyOperation,
    OprRemove = ToolbarRuleOperation.RemoveOperation,
    [PrivateApi]
    OprUnknown = ToolbarRuleOperation.UnknownOperation,
    [PrivateApi]
    OprNone = ToolbarRuleOperation.NoOperation,
}

/// <summary>
/// This is just a documentation class to show all possible values for a `operation` parameter.
///
/// _WARNING_ Do not reference this object, it can change at any time.
/// It's only here for documentation. 
/// </summary>
internal class ToolbarRuleOperation
{
    /// <summary>
    /// Symbol to make sure a button is explicitly added.
    /// Useful to force when a button would otherwise remain hidden because of a permission or another condition. 
    /// </summary>
    protected internal const char AddOperation = '+';

    /// <summary>
    /// Symbol to add a button but still respect it's internal show/hide conditions. 
    /// </summary>
    protected internal const char AutoOperation = '±';

    /// <summary>
    /// Symbol to modify a button in a toolbar - for example to force it to show.
    /// </summary>
    protected internal const char ModifyOperation = '%';

    /// <summary>
    /// Symbol to remove a button in a toolbar.
    /// </summary>
    protected internal const char RemoveOperation = '-';

    protected internal const char UnknownOperation = '¿';

    protected internal const char NoOperation = ' ';

    protected internal const char SkipInclude = '^';

    /// <summary>
    /// Verb to make sure a button is explicitly added.
    /// Useful to force when a button would otherwise remain hidden because of a permission or another condition. 
    /// </summary>
    protected internal const string AddVerb = "add";

    /// <summary>
    /// Verb to add a button but still respect it's internal show/hide conditions. 
    /// </summary>
    protected internal const string AutoVerb = "auto";

    /// <summary>
    /// Verb to modify a button in a toolbar - for example to force it to show.
    /// </summary>
    protected internal const string ModifyVerb = "modify";

    /// <summary>
    /// Verb to remove a button in a toolbar.
    /// </summary>
    protected internal const string RemoveVerb = "remove";

    internal static Dictionary<string, ToolbarRuleOps> ToolbarRuleOpSynonyms =
        new(StringComparer.InvariantCultureIgnoreCase)
        {
            { ModifyVerb, OprModify },
            { AddVerb, OprAdd },
            { AutoVerb, OprAuto },
            { RemoveVerb, OprRemove },
        };

    //internal static char FindInFlags(string flags, ToolbarRuleOperations defOp)
    //{
    //    if (!flags.HasValue()) return (char)defOp;

    //    var parts = flags.Split(',');
    //    foreach (var f in parts)
    //    {
    //        var maybeOp = Pick(f, OprUnknown);
    //        if (maybeOp != (char)OprUnknown) return maybeOp;
    //    }

    //    return (char)defOp;
    //}

    internal static char Pick(string op, ToolbarRuleOps defOp, bool? condition = default)
        => condition == false ? SkipInclude : PrePick(op, defOp);

    private static char PrePick(string op, ToolbarRuleOps defOp)
    {
        if (!op.HasValue()) return (char)defOp;
        op = op.Trim();

        if (op.Length == 1 && Enum.IsDefined(typeof(ToolbarRuleOps), (int)op[0]))
            return op[0];


        if (ToolbarRuleOpSynonyms.TryGetValue(op, out var foundSyn))
            return (char)foundSyn;

        return (char)defOp;
    }

}