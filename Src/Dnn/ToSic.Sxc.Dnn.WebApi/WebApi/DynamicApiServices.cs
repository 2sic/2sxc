using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.LookUp;

namespace ToSic.Sxc.WebApi
{
    /// <summary>
    /// Note: normally dependencies are Constructor injected.
    /// This doesn't work in DNN.
    /// But for consistency, we're building a comparable structure here.
    /// </summary>
    public class DynamicApiServices : MyServicesBase
    {
        public LazySvc<AppConfigDelegate> AppConfigDelegateLazy { get; }
        public LazySvc<Apps.App> AppOverrideLazy { get; }
        public DnnCodeRootFactory DnnCodeRootFactory { get; }
        public DnnAppFolderUtilities AppFolderUtilities { get; }

        public DynamicApiServices(
            DnnCodeRootFactory dnnCodeRootFactory,
            DnnAppFolderUtilities appFolderUtilities,
            LazySvc<Apps.App> appOverrideLazy,
            LazySvc<AppConfigDelegate> appConfigDelegateLazy)
        {
            ConnectServices(
                DnnCodeRootFactory = dnnCodeRootFactory,
                AppFolderUtilities = appFolderUtilities,
                AppOverrideLazy = appOverrideLazy,
                AppConfigDelegateLazy = appConfigDelegateLazy
            );
        }
    }
}
