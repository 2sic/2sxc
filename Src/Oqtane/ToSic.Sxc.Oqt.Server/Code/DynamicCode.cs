using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Oqt.Server.Code
{
    // TODO: remove, because we will use dynamic code hybrid implementation
    // ReSharper disable once UnusedMember.Global
    /// <summary>
    /// This is a base class for custom code files with context. <br/>
    /// If you create a class file for dynamic use and inherit from this, then the compiler will automatically add objects like Link, Dnn, etc.
    /// The class then also has AsDynamic(...) and AsList(...) commands like a normal razor page.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract class DynamicCode : Sxc.Code.DynamicCode, IOqtaneDynamicCode //, IHasOqtaneDynamicCodeContext
    {
        ///// <inheritdoc />
        ///// TODO: WIP
        //public IOqtContext Oqt => DynCode?.Oqt;

        //[PrivateApi] public OqtaneDynamicCodeRoot _DynCodeRoot => (UnwrappedContents as IHasOqtaneDynamicCodeContext)?._DynCodeRoot;
    }
}
