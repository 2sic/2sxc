using ToSic.Sxc.Apps;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// WIP
/// </summary>
public interface ICodeTypedApiHelper: ICodeAnyApiHelper
{
    #region Content, Header, App, Data, Resources, Settings

    IAppTyped AppTyped { get; }

    #endregion

    ServiceKit16 ServiceKit16 { get; }
}