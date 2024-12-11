using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Web.Internal.PageFeatures;

namespace ToSic.Sxc.Web.Internal.PageService;

partial class PageService
{
    /// <inheritdoc />
    public string TurnOn(object runOrSpecs,
        NoParamOrder noParamOrder = default,
        object require = default,
        object data = default,
        IEnumerable<object> args = default,
        bool condition = true,
        bool? noDuplicates = default,
        string addContext = default
    )
    {
        var l = Log.Fn<string>($"{runOrSpecs}: {runOrSpecs}; {require}; {data}");

        // Check condition - default is true - so if it's false, this overload was called
        if (!condition)
            return l.ReturnNull("condition false");

        // first activate the page feature
        Activate(SxcPageFeatures.TurnOn.NameId);

        // then generate the turn-on and add to module state
        var tag = turnOn.Value.Run(runOrSpecs, require: require, data: data, args: args, addContext: addContext);

#if NETCOREAPP
        // In the Oqtane Interactive Server, the Dependency Injection (DI) session scope is bound to the first HTTP request of the user's browser session,
        // and it does not change during subsequent SignalR communications (until a full page reload).
        // As a result, scoped services have the same instance for all 2sxc module instances across all pages during a user's browser session.
        // To prevent conflicts, we need to add the ModuleId to the ModuleService to scope its functionality to each module rendering.
        moduleService.Value.AddToMore(tag, noDuplicates: noDuplicates == true, moduleId: GetModuleId());
#else
        moduleService.Value.AddToMore(tag, noDuplicates: noDuplicates == true);
#endif

        // Then return empty string
        return l.ReturnAsOk(null);
    }

#if NETCOREAPP
    /// <summary>
    /// Retrieves the Module ID for the current module rendering context.
    /// </summary>
    /// <returns>The resolved Module ID as an integer; returns zero if it cannot be determined.</returns>
    private int GetModuleId()
    {
        var moduleId = contextOfBlock.Module.Id; // This is not working. It's always 0.
        if (moduleId != default || accessor.HttpContext == null) return moduleId;

        //accessor.HttpContext.Items.TryGetValue("Oqtane.Sxc.RenderParametersCounter", out var counter);
        if (accessor.HttpContext.Items.TryGetValue(ModuleService.OqtaneSxcRenderParameters, out var @params))
            moduleId = (@params as dynamic)?.ModuleId ?? moduleId;

        return moduleId;
    }
#endif
}