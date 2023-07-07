using ToSic.Lib.Documentation;
using ToSic.Sxc;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Run;

// ReSharper disable once CheckNamespace
namespace Custom.Dnn
{
    /// <summary>
    /// This is the base class for custom code (.cs) files in your Apps.
    /// By inheriting from this base class, you will automatically have the context like the App object etc. available. 
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract class Code12 : DynamicCode12, IHasDnn
    {
        /// <inheritdoc />
        public IDnnContext Dnn => (_DynCodeRoot as IHasDnn)?.Dnn;

        [PrivateApi] public override int CompatibilityLevel => Constants.CompatibilityLevel12;

    }
}
