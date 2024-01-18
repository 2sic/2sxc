using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Internal;

[PrivateApi("v14")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IHasKit<out TServiceKit> where TServiceKit : ServiceKit
{
    /// <summary>
    /// The Service Kit containing all kinds of services which are commonly used.
    /// The services on the Kit are context-aware, so they know what App is currently being used etc.
    /// </summary>
    TServiceKit Kit { get; }
}