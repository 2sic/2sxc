using ToSic.Lib.Coding;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using Attribute = ToSic.Razor.Markup.Attribute;

namespace ToSic.Sxc.Services;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class TurnOnService: ServiceBase, ITurnOnService
{
    private const string TagName = "turnOn";
    private const string AttributeName = "turn-on";

    public TurnOnService(LazySvc<IHtmlTagsService> htmlTagsService) : base(Constants.SxcLogName + ".TrnOnS")
    {
        ConnectServices(
            _htmlTagsService = htmlTagsService
        );
    }
    private readonly LazySvc<IHtmlTagsService> _htmlTagsService;

    // TODO:
    // - TEST
    // - CREATE LOG INTERCEPT - probably only exist on the FN method right now?

    public Attribute Attribute(
        object runOrSpecs,
        NoParamOrder noParamOrder = default,
        object require = default,
        object data = default
    )
    {
        var l = Log.Fn<Attribute>();
        var specs = PickOrBuildSpecs(runOrSpecs, require, data);
        var attr = _htmlTagsService.Value.Attr(AttributeName, specs);
        return l.ReturnAsOk(attr);
    }

    public IHtmlTag Run(
        object runOrSpecs,
        NoParamOrder noParamOrder = default,
        object require = default,
        object data = default
    )
    {
        var l = Log.Fn<IHtmlTag>();
        var specs = PickOrBuildSpecs(runOrSpecs, require, data);
        var tag = _htmlTagsService.Value.Custom(TagName).Attr(AttributeName, specs);
        return l.ReturnAsOk(tag);
    }

    private static object PickOrBuildSpecs(object runOrSpecs, object require, object data)
    {
        if (!(runOrSpecs is string run)) return runOrSpecs;

        if (require is null && data is null) return new { run };
        if (require is null) return new { run, data };
        if (data is null) return new { run, require };

        return new { run, require, data };
    }
}