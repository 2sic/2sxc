using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Services.Internal;
using Attribute = ToSic.Razor.Markup.Attribute;

namespace ToSic.Sxc.Services;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class TurnOnService(LazySvc<IHtmlTagsService> htmlTagsService)
    : ServiceBase(SxcLogName + ".TrnOnS", connect: [htmlTagsService]), ITurnOnService
{
    private const string TagName = "turnOn";
    private const string AttributeName = "turn-on";

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
        var attr = htmlTagsService.Value.Attr(AttributeName, specs);
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
        var tag = htmlTagsService.Value.Custom(TagName).Attr(AttributeName, specs);
        return l.ReturnAsOk(tag);
    }

    private static object PickOrBuildSpecs(object runOrSpecs, object require, object data)
    {
        if (runOrSpecs is not string run) return runOrSpecs;

        if (require is null && data is null) return new { run };
        if (require is null) return new { run, data };
        if (data is null) return new { run, require };

        return new { run, require, data };
    }
}