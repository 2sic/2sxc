using ToSic.Eav.Context;
using ToSic.Eav.Data.Build;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Data.AsConverter
{
    // todo: make internal once we have an interface
    public partial class AsConverterService: ServiceForDynamicCode
    {
        private readonly LazySvc<DynamicWrapperFactory> _dynJacketFactory;
        private readonly LazySvc<DataBuilder> _dataBuilderLazy;
        private readonly LazySvc<DynamicEntity.MyServices> _dynamicEntityDependenciesLazy;
        private readonly LazySvc<AdamManager> _adamManagerLazy;
        private readonly LazySvc<IContextOfApp> _contextOfAppLazy;

        public AsConverterService(
            LazySvc<DynamicEntity.MyServices> dynamicEntityDependencies,
            LazySvc<AdamManager> adamManager,
            LazySvc<IContextOfApp> contextOfApp,
            LazySvc<DataBuilder> dataBuilderLazy,
            LazySvc<DynamicWrapperFactory> dynJacketFactory) : base("Sxc.AsConv")
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
            _adamManagerPrepared = adamManagerPrepared;
        }

        private ISite _siteOrNull;
        public int CompatibilityLevel => _priorityCompatibilityLevel ?? _compatibilityLevel;
        private int? _priorityCompatibilityLevel;
        private int _compatibilityLevel = Constants.CompatibilityLevel10;
        private AdamManager _adamManagerPrepared;


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






        //private ITypedRead AsTypedInternal(object dynObject)
        //{
        //    var l = Log.Fn<ITypedRead>();
        //    switch (dynObject)
        //    {
        //        //case null:
        //        //    return l.Return(AsDynamicFromJson(null), "null");
        //        //case string strObject:
        //        //    return l.Return(AsDynamicFromJson(strObject), "string");
        //        //case IDynamicEntity dynEnt:
        //        //    return l.Return(dynEnt, "DynamicEntity");
        //        //// New case - should avoid re-converting dynamic json, DynamicStack etc.
        //        //case ISxcDynamicObject sxcDyn:
        //        //    return l.Return(sxcDyn, "Dynamic Something");
        //        //case IEntity entity:
        //        //    return l.Return(new DynamicEntity(entity, DynamicEntityServices), "IEntity");


        //        //case DynamicObject typedDynObject:
        //        //    return wrapLog.Return(typedDynObject, "DynamicObject");
        //        default:
        //            // Check value types - note that it won't catch strings, but these were handled above
        //            //if (dynObject.GetType().IsValueType) return wrapLog.Return(dynObject, "bad call - value type");

        //            // 2021-09-14 new - just convert to a DynamicReadObject
        //            var result = DynamicHelpers.WrapIfPossible(dynObject, true, true, false);
        //            if (result is ITypedRead resTyped) return l.Return(resTyped, "converted to dyn-read");

        //            //// Note 2dm 2021-09-14 returning the original object was actually the default till now.
        //            //// Unknown conversion, just return the original and see what happens/breaks
        //            //// probably not a good solution
        //            return l.Return(null, "unknown, return original");
        //    }
        //}
    }
}
