using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Dnn.Integration;

namespace ToSic.Sxc.Dnn.WebApi.Internal;

/// <summary>
/// Note: normally dependencies are Constructor injected.
/// This doesn't work in DNN.
/// But for consistency, we're building a comparable structure here.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ApiControllerMyServices(
    ICodeApiServiceFactory codeApiServiceFactory,
    DnnAppFolderUtilities appFolderUtilities,
    LazySvc<Apps.App> appOverrideLazy)
    : MyServicesBase(connect: [codeApiServiceFactory, appFolderUtilities, appOverrideLazy])
{
    public LazySvc<Apps.App> AppOverrideLazy { get; } = appOverrideLazy;
    public ICodeApiServiceFactory CodeApiServiceFactory { get; } = codeApiServiceFactory;
    public DnnAppFolderUtilities AppFolderUtilities { get; } = appFolderUtilities;
}