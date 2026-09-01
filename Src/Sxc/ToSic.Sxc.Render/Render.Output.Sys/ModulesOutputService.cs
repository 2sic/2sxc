namespace ToSic.Sxc.Render.Output.Sys;

/// <summary>
/// Provides functionality to manage module rendering and HTML tag collections,
/// ensuring proper scoping of services for each module instance (sometimes difficult in Oqtane applications).
/// </summary>
/// <remarks>
/// IModuleService is registered as a scoped service.
/// In the Oqtane Interactive Server, the Dependency Injection (DI) session scope is bound to the first HTTP request
/// of the user's browser session and remains unchanged during subsequent SignalR communications (until a full page reload).
/// Consequently, scoped services share the same instance for all 2sxc module instances across all pages during a user's session.
/// To prevent conflicts, the `ModuleId` is used to scope the `ModuleService` functionality to each module rendering.
/// </remarks>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal partial class ModulesOutputService() : ServiceBase(SxcLogName + ".ModSvc"), IModulesOutputService;
