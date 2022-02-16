using ToSic.Eav.Documentation;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.Run;

// ReSharper disable once CheckNamespace
namespace Custom.Dnn
{
    /// <summary>
    /// This is the base class for custom code (.cs) files in your Apps.
    /// By inheriting from this base class, you will automatically have the context like the App object etc. available. 
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract class Code12 : ToSic.Sxc.Code.DynamicCode12, IDnnDynamicCodeAdditions
    {
        /// <inheritdoc />
        public IDnnContext Dnn => (_DynCodeRoot as DnnDynamicCodeRoot)?.Dnn;
    }
}
