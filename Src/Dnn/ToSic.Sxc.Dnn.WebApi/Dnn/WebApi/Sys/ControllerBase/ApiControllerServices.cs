using ToSic.Sxc.Dnn.Integration;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Dnn.WebApi.Sys;

/// <summary>
/// Note: normally dependencies are Constructor injected.
/// This doesn't work in DNN.
/// But for consistency, we're building a comparable structure here.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public record ApiControllerDependencies(
    IExecutionContextFactory ExCtxFactory,
    DnnAppFolderUtilities AppFolderUtilities,
    LazySvc<Apps.App> AppOverrideLazy)
    : DependenciesRecord(connect: [ExCtxFactory, AppFolderUtilities, AppOverrideLazy]);