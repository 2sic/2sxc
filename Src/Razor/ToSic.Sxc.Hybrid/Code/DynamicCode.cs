using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Hybrid.Code
{
    // ReSharper disable once UnusedMember.Global
    /// <summary>
    /// This is a base class for custom code files with context. <br/>
    /// If you create a class file for dynamic use and inherit from this, then the compiler will automatically add objects like Link, Dnn, etc.
    /// The class then also has AsDynamic(...) and AsList(...) commands like a normal razor page.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract class DynamicCode : Sxc.Code.DynamicCode, IHybridDynamicCode, IHasHybridDynamicCodeContext
    {
        /// <inheritdoc />
        /// TODO: WIP
        //public IOqtContext Oqt => DynCode?.Oqt;

        [PrivateApi] public HybridDynamicCode DynCode => (UnwrappedContents as IHasHybridDynamicCodeContext)?.DynCode;
    }
}
