using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Sys.BlockBuilder;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services.Sys.CodeApiServiceHelpers;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Users;
using App = ToSic.Sxc.Apps.App;

namespace ToSic.Sxc.Services.Sys.DynamicCodeService;

/// <summary>
/// WIP - goal is to have a DI factory which creates DynamicCode objects for use in Skins and other external controls
/// Not sure how to get this to work, since normally we always start with a code-file, and here we don't have one!
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class DynamicCodeService(CodeApiServiceBase.Dependencies services, string? logName = null /* must be nullable for DI */)
    : CodeApiServiceBase(services, logName ?? $"{SxcLogName}.DCS"),
        IDynamicCodeService
{
    #region Constructor and Init

    public class ScopedDependencies(
        Generator<IExecutionContextFactory> codeRootGenerator,
        Generator<App> appGenerator,
        LazySvc<IModuleAndBlockBuilder> modAndBlockBuilder)
        : DependenciesBase(connect: [codeRootGenerator, appGenerator, modAndBlockBuilder])
    {
        public Generator<App> AppGenerator { get; } = appGenerator;
        public Generator<IExecutionContextFactory> CodeRootGenerator { get; } = codeRootGenerator;
        public LazySvc<IModuleAndBlockBuilder> ModAndBlockBuilder { get; } = modAndBlockBuilder;
    }

    /// <summary>
    /// This is for all the services used here, or also for services needed in inherited classes which will need the same scoped objects.
    /// It's important to understand that everything in here will use the scoped service provider.
    /// </summary>
    [field: AllowNull, MaybeNull]
    protected IServiceProvider ScopedServiceProvider => field ??= Services.ServiceProvider.CreateScope().ServiceProvider;

    [field: AllowNull, MaybeNull]
    private ScopedDependencies ServicesScoped => field ??= ScopedServiceProvider.Build<ScopedDependencies>().ConnectServices(Log);


    protected void ActivateEditUi() => EditUiRequired = true;

    protected bool EditUiRequired;

    #endregion

}