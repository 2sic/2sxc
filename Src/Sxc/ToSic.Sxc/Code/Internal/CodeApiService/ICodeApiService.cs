using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Data;
using CodeDataFactory = ToSic.Sxc.Data.Internal.CodeDataFactory;

namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// This is the same as IDynamicCode, but the root object. 
/// We create another interface to ensure we don't accidentally pass around a sub-object where the root is really needed.
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ICodeApiService : IDynamicCode12
{
    //[PrivateApi("WIP")]
    //IBlock _Block { get; }
    //[PrivateApi]
    //void AttachApp(IApp app);

    new IDynamicStack Resources { get; }
    new IDynamicStack Settings { get; }

    #region AsConverter (internal)

    [PrivateApi("internal use only")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    CodeDataFactory _Cdf { get; }

    #endregion
}

internal interface ICodeApiServiceInternal
{
    [PrivateApi]
    void AttachApp(IApp app);

    [PrivateApi("WIP")]
    IBlock _Block { get; }

}