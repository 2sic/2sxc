using ToSic.Eav.Sys;

namespace ToSic.Sxc.Cms.Users.Sys;

public class UserConstants
{
    #region Constant user objects for Unknown/Anonymous

    public static readonly UserModel AnonymousUser = new()
    {
        Id = -1,
        Name = SxcUserConstants.Anonymous,
        Roles = [],
    };

    public static readonly UserModel UnknownUser = new()
    {
        Id = -2,
        Name = EavConstants.NullNameId,
        Roles = [],
    };

    #endregion

    #region Other Constants

    internal const char Separator = ',';
    internal const int NullInteger = -1;
    public const string IncludeRequired = "required";
    public const string IncludeOptional = "true";
    public const string IncludeForbidden = "false";

    #endregion
}