using ToSic.Eav.Documentation;
using ToSic.Sxc.Dnn.Code;

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// Marks objects which have DynCode - the connection to the Dnn and App objects
    /// </summary>
    [PrivateApi]
    public interface INeedsDynCodeContext
    {
        [PrivateApi("internal, for passing around context!")]
        DnnDynamicCodeRoot DynCode { set; }

    }
}
