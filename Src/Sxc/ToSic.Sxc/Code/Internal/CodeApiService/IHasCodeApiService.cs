namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// Marks objects which have DynCodeRoot which is passed around to sub-objects as needed
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IHasCodeApiService
{
    /// <summary>
    /// The dynamic code root which many dynamic code objects need to access prepared context, state etc.
    /// </summary>
    [PrivateApi("internal, for passing around context!")]
    // ReSharper disable once InconsistentNaming
    ICodeApiService _CodeApiSvc { get; }
}