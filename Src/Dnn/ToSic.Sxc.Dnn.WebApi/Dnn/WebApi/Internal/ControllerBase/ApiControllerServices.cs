using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Dnn.Integration;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Dnn.WebApi.Internal;

/// <summary>
/// Note: normally dependencies are Constructor injected.
/// This doesn't work in DNN.
/// But for consistency, we're building a comparable structure here.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ApiControllerMyServices(
    IExecutionContextFactory exCtxFactory,
    DnnAppFolderUtilities appFolderUtilities,
    LazySvc<Apps.App> appOverrideLazy)
    : MyServicesBase(connect: [exCtxFactory, appFolderUtilities, appOverrideLazy])
{
    public LazySvc<Apps.App> AppOverrideLazy { get; } = appOverrideLazy;
    public IExecutionContextFactory ExecutionContextFactory { get; } = exCtxFactory;
    public DnnAppFolderUtilities AppFolderUtilities { get; } = appFolderUtilities;
}