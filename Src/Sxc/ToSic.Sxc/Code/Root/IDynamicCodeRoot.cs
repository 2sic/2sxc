using ToSic.Lib.Documentation;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code.Helpers;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// This is the same as IDynamicCode, but the root object. 
    /// We create another interface to ensure we don't accidentally pass around a sub-object where the root is really needed.
    /// </summary>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public interface IDynamicCodeRoot : IDynamicCode12
    {
        [PrivateApi("WIP")] IBlock Block { get; }

        [PrivateApi] DynamicCodeDataSources DataSources { get; }

        [PrivateApi] void AttachApp(IApp app);


        new DynamicStack Resources { get; }
        new DynamicStack Settings { get; }

        #region AsConverter (internal)

        [PrivateApi("internal use only")]
        CodeDataFactory Cdf { get; }

        #endregion
    }
}
