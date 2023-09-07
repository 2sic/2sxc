using ToSic.Eav.Code.InfoSystem;
using ToSic.Eav.Context;
using ToSic.Eav.Data.Build;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data.Wrapper;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Data
{
    // todo: make internal once we have an interface
    public partial class CodeDataFactory: ServiceForDynamicCode
    {
        private readonly LazySvc<CodeInfoService> _codeInfoSvc;
        private readonly Generator<CodeJsonWrapper> _wrapJsonGenerator;
        private readonly LazySvc<CodeDataServices> _codeDataServices;
        private readonly LazySvc<CodeDataWrapper> _codeDataWrapper;
        private readonly LazySvc<DataBuilder> _dataBuilderLazy;
        private readonly LazySvc<AdamManager> _adamManagerLazy;
        private readonly LazySvc<IContextOfApp> _contextOfAppLazy;

        public CodeDataFactory(
            LazySvc<CodeDataServices> codeDataServices,
            LazySvc<AdamManager> adamManager,
            LazySvc<IContextOfApp> contextOfApp,
            LazySvc<DataBuilder> dataBuilderLazy,
            LazySvc<CodeDataWrapper> codeDataWrapper,
            Generator<CodeJsonWrapper> wrapJsonGenerator,
            LazySvc<CodeInfoService> codeInfoSvc) : base("Sxc.AsConv")
        {
            ConnectServices(
                _codeDataServices = codeDataServices,
                _adamManagerLazy = adamManager,
                _contextOfAppLazy = contextOfApp,
                _dataBuilderLazy = dataBuilderLazy,
                _codeDataWrapper = codeDataWrapper,
                _wrapJsonGenerator = wrapJsonGenerator,
                _codeInfoSvc = codeInfoSvc
            );
        }

        internal CodeInfoService CodeInfo => _codeInfoSvc.Value;

        public void SetCompatibilityLevel(int compatibilityLevel) => _priorityCompatibilityLevel = compatibilityLevel;

        public void SetFallbacks(ISite site, int? compatibility = default, AdamManager adamManagerPrepared = default)
        {
            _siteOrNull = site;
            _compatibilityLevel = compatibility ?? _compatibilityLevel;
            _adamManager.Reset(adamManagerPrepared);
        }

        private ISite _siteOrNull;
        public int CompatibilityLevel => _priorityCompatibilityLevel ?? _compatibilityLevel;
        private int? _priorityCompatibilityLevel;
        private int _compatibilityLevel = Constants.CompatibilityLevel10;


        #region CodeDataServices

        public CodeDataServices Services => _codeDataServices.Value;

        // If we don't have a DynCodeRoot, try to generate the language codes and compatibility
        // There are cases where these were supplied using SetFallbacks, but in some cases none of this is known
        internal string[] Dimensions => _dimensions.Get(() => _DynCodeRoot?.CmsContext.SafeLanguagePriorityCodes()
                                                              ?? _siteOrNull.SafeLanguagePriorityCodes());
        private readonly GetOnce<string[]> _dimensions = new GetOnce<string[]>();

        internal IBlock BlockOrNull => _DynCodeRoot?.Block;

        #endregion

        public DynamicJacketBase Json2Jacket(string json, string fallback = default)
            => _wrapJsonGenerator.New().Setup(WrapperSettings.Dyn(true, true))
                .Json2Jacket(json, fallback: fallback);

    }
}
