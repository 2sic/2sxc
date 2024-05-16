using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.Internal.Url;
using static ToSic.Sxc.Edit.Toolbar.EntityEditInfo;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar.Internal;

partial class ToolbarBuilder
{
    public IToolbarBuilder Layout(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    ) => AddAdminAction("layout", noParamOrder, ui, parameters, operation, target, tweak);


    public IToolbarBuilder Code(
        object target,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    )
    {
        // If we don't have a tweak then create the params the classic way
        if (tweak == null)
        {
            var paramsWithCode = new ObjectToUrl().SerializeWithChild(parameters, (target as string).HasValue() ? "call=" + target : "", "");
            return AddAdminAction("code", noParamOrder, ui, paramsWithCode, operation, target, tweak);
        }

        // if we have a tweak, we must place the call into that to avoid an error that parameters & tweak are provided
        ITweakButton ReTweak(ITweakButton _) => tweak(new TweakButton().Parameters("call", target?.ToString()));
        return AddAdminAction("code", noParamOrder, ui, parameters, operation, target, ReTweak);
    }

    public IToolbarBuilder Fields(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    )
    {
        var pars = PreCleanParams(tweak, defOp: OprAdd, operation: operation, ui: ui, parameters: parameters);
        return EntityRule("fields", target, pars, propsKeep: [KeyContentType], contentType: target as string).Builder;
    }


    public IToolbarBuilder Template(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    ) => AddAdminAction("template", noParamOrder, ui, parameters, operation, target, tweak);

    public IToolbarBuilder Query(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    ) => AddAdminAction("query", noParamOrder, ui, parameters, operation, target, tweak);

    public IToolbarBuilder View(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    ) => AddAdminAction("view", noParamOrder, ui, parameters, operation, target, tweak);

    public IToolbarBuilder Edition(
        NoParamOrder noParamOrder = default,
        string editions = default,
        Func<ITweakButton, ITweakButton> tweak = default
    )
    {
        var paramsMergeInTweak = editions == default ? null : new { editions };
        return AddAdminAction("edition", noParamOrder, null, paramsMergeInTweak, null, null, tweak);
    }
}