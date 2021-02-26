using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Hybrid.Code
{
    /// <summary>
    /// Marks objects which have DynCode - the connection to the Oqt and App objects
    /// </summary>
    [PrivateApi]
    public interface IHasHybridDynamicCodeContext
    {
        [PrivateApi("internal, for passing around context!")]
        HybridDynamicCode DynCode { get; }
    }
}
