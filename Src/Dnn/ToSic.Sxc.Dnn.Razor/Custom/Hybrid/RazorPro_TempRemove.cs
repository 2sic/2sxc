using ToSic.Eav.Code.InfoSystem;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class RazorPro
    {

        #region Killed DynamicModel and new TypedModel

        //[PrivateApi("Hide as it's nothing that should be used")]
        //public new object DynamicModel => throw new NotSupportedException($"{nameof(DynamicModel)} isn't supported in v16 typed. Use TypedModel instead.");

        #endregion

        //[PrivateApi("WIP 16.02 - to be removed")]
        //public ITypedModel TypedModel => CcS.GetAndWarn(DynamicCode16Warnings.NoTypedModel, MyModel);

    }


}
