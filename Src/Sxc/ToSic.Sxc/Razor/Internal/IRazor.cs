using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;

namespace ToSic.Sxc.Razor.Internal;

[PrivateApi("not sure where/if it goes anywhere")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IRazor: IHasCodeApiService, INeedsCodeApiService
{
    /// <summary>
    /// The path to this Razor WebControl.
    /// This is for consistency, because asp.net Framework has a property "VirtualPath" whereas .net core uses "Path"
    /// From now on it should always be Path for cross-platform code
    /// </summary>
    [PublicApi("This is a polyfill to ensure the old Razor has the same property as .net Core Razor")]
    string Path { get; }

}