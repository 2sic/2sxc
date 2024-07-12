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
    protected virtual string TagName => "turnOn";
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
        var specs = PickOrBuildSpecs(runOrSpecs: runOrSpecs, require: require, data: data, args: null, addContext: null);
        var attr = htmlTagsService.Value.Attr(AttributeName, specs);
        return l.ReturnAsOk(attr);
    }

    public IHtmlTag Run(
        object runOrSpecs,
        NoParamOrder noParamOrder = default,
        object require = default,
        object data = default,
        IEnumerable<object> args = default,
        string addContext = default
    )
    {
        var l = Log.Fn<IHtmlTag>();
        var specs = PickOrBuildSpecs(runOrSpecs: runOrSpecs, require: require, data: data, args: args, addContext: addContext);
        var tag = htmlTagsService.Value.Custom(TagName).Attr(AttributeName, specs);
        return l.ReturnAsOk(tag);
    }

    internal static object PickOrBuildSpecs(object runOrSpecs, object require, object data, IEnumerable<object> args, string addContext)
        => runOrSpecs is not string run
            // if we already have a full configuration, just return it
            ? runOrSpecs
            // otherwise build a new one
            : new TurnOnSpecs
            {
                Args = args,
                AddContext = addContext,
                Data = data,
                Require = require,
                Run = run,
            };
}