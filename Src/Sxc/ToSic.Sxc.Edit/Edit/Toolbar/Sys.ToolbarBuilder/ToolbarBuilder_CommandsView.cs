using ToSic.Sxc.Web.Sys.Url;
using ToSic.Sys.Utils;
using static ToSic.Sxc.Edit.Toolbar.Sys.EntityEditInfo;
using static ToSic.Sxc.Edit.Toolbar.Sys.Rules.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar.Sys.ToolbarBuilder;

partial record ToolbarBuilder
{
    public IToolbarBuilder Layout(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddAdminAction("layout", npo, ui, parameters, operation, target, tweak);


    public IToolbarBuilder Code(
        object target,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    )
    {
        // If we don't have a tweak then create the params the classic way
        if (tweak == null)
        {
            var paramsWithCode = new ObjectToUrl().SerializeWithChild(parameters, (target as string).HasValue() ? "call=" + target : "", "");
            return AddAdminAction("code", npo, ui, paramsWithCode, operation, target, tweak);
        }

        // if we have a tweak, we must place the call into that to avoid an error that parameters & tweak are provided
        ITweakButton ReTweak(ITweakButton _) => tweak(new TweakButton.TweakButton().Parameters("call", target?.ToString()));
        return AddAdminAction("code", npo, ui, parameters, operation, target, ReTweak);
    }

    public IToolbarBuilder Fields(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    )
    {
        var pars = PreCleanParams(tweak, defOp: OprAdd, operation: operation, ui: ui, parameters: parameters);
        return EntityRule("fields", target, pars, propsKeep: [KeyContentType], contentType: target as string).Builder;
    }


    public IToolbarBuilder Template(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddAdminAction("template", npo, ui, parameters, operation, target, tweak);

    public IToolbarBuilder Query(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddAdminAction("query", npo, ui, parameters, operation, target, tweak);

    public IToolbarBuilder View(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddAdminAction("view", npo, ui, parameters, operation, target, tweak);

    public IToolbarBuilder Edition(
        NoParamOrder npo = default,
        string? editions = default,
        Func<ITweakButton, ITweakButton>? tweak = default
    )
    {
        var paramsMergeInTweak = editions == default ? null : new { editions };
        return AddAdminAction("edition", npo, null, paramsMergeInTweak, null, null, tweak);
    }
}