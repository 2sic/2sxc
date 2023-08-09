using ToSic.Eav.Context;
using ToSic.Eav.Data.Build;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data.Wrapper;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Data
{
    // todo: make internal once we have an interface
    public partial class CodeDataFactory: ServiceForDynamicCode
    {
        private readonly LazySvc<CodeDataWrapper> _dynJacketFactory;
        private readonly LazySvc<DataBuilder> _dataBuilderLazy;
        private readonly LazySvc<DynamicEntity.MyServices> _dynamicEntityDependenciesLazy;
        private readonly LazySvc<AdamManager> _adamManagerLazy;
        private readonly LazySvc<IContextOfApp> _contextOfAppLazy;

        public CodeDataFactory(
            LazySvc<DynamicEntity.MyServices> dynamicEntityDependencies,
            LazySvc<AdamManager> adamManager,
            LazySvc<IContextOfApp> contextOfApp,
            LazySvc<DataBuilder> dataBuilderLazy,
            LazySvc<CodeDataWrapper> dynJacketFactory) : base("Sxc.AsConv")
        {
            ConnectServices(
                _dynamicEntityDependenciesLazy = dynamicEntityDependencies,
                _adamManagerLazy = adamManager,
                _contextOfAppLazy = contextOfApp,
                _dataBuilderLazy = dataBuilderLazy,
                _dynJacketFactory = dynJacketFactory
            );
        }

        public void SetCompatibilityLevel(int compatibilityLevel) => _priorityCompatibilityLevel = compatibilityLevel;

        public void SetFallbacks(ISite site, int? compatibility = default, AdamManager adamManagerPrepared = default)
        {
            _siteOrNull = site;
            _compatibilityLevel = compatibility ?? _compatibilityLevel;
            //_adamManagerPrepared = adamManagerPrepared;
            _adamManager.Reset(adamManagerPrepared);
        }

        private ISite _siteOrNull;
        public int CompatibilityLevel => _priorityCompatibilityLevel ?? _compatibilityLevel;
        private int? _priorityCompatibilityLevel;
        private int _compatibilityLevel = Constants.CompatibilityLevel10;
        //private AdamManager _adamManagerPrepared;


        #region Kit - used by some things created by ASC

        public ServiceKit14 GetServiceKit14() => _DynCodeRoot.GetKit<ServiceKit14>();

        #endregion

        #region DynamicEntityServices

        private DynamicEntity.MyServices DynamicEntityServices => _dynamicEntityServices.Get(Log, l =>
        {
            var rawServices = _dynamicEntityDependenciesLazy.Value;

            // Get block or if not known, null it
            var block = _DynCodeRoot?.Block;

            // If we don't have a DynCodeRoot, try to generate the language codes and compatibility
            // There are cases where these were supplied using SetFallbacks, but in some cases none of this is known
            var languageCodes = _DynCodeRoot?.CmsContext.SafeLanguagePriorityCodes()
                                ?? _siteOrNull.SafeLanguagePriorityCodes();

            return rawServices.Init(block, languageCodes, this);
        });
        private readonly GetOnce<DynamicEntity.MyServices> _dynamicEntityServices = new GetOnce<DynamicEntity.MyServices>();

        #endregion

        public DynamicJacketBase AsDynamicFromJson(string json, string fallback = default) 
            => _dynJacketFactory.Value.FromJson(json, fallback);

    }
}
