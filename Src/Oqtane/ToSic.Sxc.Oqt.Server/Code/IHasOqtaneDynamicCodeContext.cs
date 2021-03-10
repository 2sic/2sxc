using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Oqt.Server.Code
{
    // TODO: remove, because we will use dynamic code hybrid implementation
    /// <summary>
    /// Marks objects which have DynCode - the connection to the Oqt and App objects
    /// </summary>
    [PrivateApi]
    public interface IHasOqtaneDynamicCodeContext
    {
        [PrivateApi("internal, for passing around context!")]
        OqtaneDynamicCode DynCode { get; }
    }
}
