using ToSic.Sxc.Context;
using ToSic.Sxc.Web.Internal.PageFeatures;

namespace ToSic.Sxc.Web.Internal.PageService;

partial class PageService
{
    /// <inheritdoc />
    public string? TurnOn(object runOrSpecs,
        NoParamOrder noParamOrder = default,
        object? require = default,
        object? data = default,
        IEnumerable<object>? args = default,
        bool condition = true,
        bool? noDuplicates = default,
        string? addContext = default
    )
    {
        var l = Log.Fn<string?>($"{runOrSpecs}: {runOrSpecs}; {require}; {data}");

        // Check condition - default is true - so if it's false, this overload was called
        if (!condition)
            return l.ReturnNull("condition false");

        // first activate the page feature
        Activate(SxcPageFeatures.TurnOn.NameId);

        // then generate the turn-on and add to module state
        var tag = turnOn.Value.Run(runOrSpecs, require: require, data: data, args: args, addContext: addContext);

        // In the Oqtane Interactive Server, the Dependency Injection (DI) session scope is bound to the first HTTP request of the user's browser session,
        // and it does not change during subsequent SignalR communications (until a full page reload).
        // As a result, scoped services have the same instance for all 2sxc module instances across all pages during a user's browser session.
        // To prevent conflicts, we need to add the ModuleId to the ModuleService to scope its functionality to each module rendering.
        // Note: in DNN, the ModuleId will be ignored.
        moduleService.Value.AddTag(tag, moduleId: ExCtx.GetState<ICmsContext>().Module.Id, noDuplicates: noDuplicates == true);

        // Then return empty string for usage as @Kit.Page.TurnOn(...)
        return l.ReturnAsOk(null);
    }
}