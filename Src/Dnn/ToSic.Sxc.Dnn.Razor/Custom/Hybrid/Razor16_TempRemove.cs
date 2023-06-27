using ToSic.Eav.Code.InfoSystem;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Razor16
    {

        #region Temporary v16 objects which must get removed again

        [PrivateApi]
        private CodeInfoService CcS => _ccs.Get(GetService<CodeInfoService>);
        private readonly GetOnce<CodeInfoService> _ccs = new GetOnce<CodeInfoService>();

        //[PrivateApi]
        //public new ITypedStack Settings => CcS.GetAndWarn(DynamicCode16Warnings.AvoidSettingsResources, _DynCodeRoot.Settings);

        //[PrivateApi]
        //public new ITypedStack Resources => CcS.GetAndWarn(DynamicCode16Warnings.AvoidSettingsResources, _DynCodeRoot.Resources);



        #endregion


        #region Killed DynamicModel and new TypedModel

        //[PrivateApi("Hide as it's nothing that should be used")]
        //public new object DynamicModel => throw new NotSupportedException($"{nameof(DynamicModel)} isn't supported in v16 typed. Use TypedModel instead.");

        #endregion

        //[PrivateApi("WIP 16.02 - to be removed")]
        //public ITypedModel TypedModel => CcS.GetAndWarn(DynamicCode16Warnings.NoTypedModel, MyModel);

        #region AsItem(s) / Merge

        /// <inheritdoc />
        public ITypedStack Merge(params object[] items) => _DynCodeRoot.AsC.MergeTyped(items);

        #endregion


        /// <inheritdoc />
        public ITypedRead Read(string json, string fallback = default) => _DynCodeRoot.AsC.AsDynamicFromJson(json, fallback);

    }


}
