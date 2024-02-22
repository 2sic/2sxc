using ToSic.Lib.Services;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Services.Internal;

/// <summary>
/// Base class to provide data to the UserService.
///
/// Must be overriden in each platform.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class UserSourceProvider(string logName) : ServiceBase(logName)
{
    public abstract string PlatformIdentityTokenPrefix { get; }

    internal abstract ICmsUser PlatformUserInformationDto(int userId, int siteId);
}