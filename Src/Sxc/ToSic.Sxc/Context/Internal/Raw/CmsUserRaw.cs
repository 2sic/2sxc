using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Models.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Context.Internal.Raw;

/// <summary>
/// Special class extending raw user data, so it can be used for CmsUser in special scenarios.
/// </summary>
[PrivateApi("this is only internal - public access is always through interface")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public record CmsUserRaw: UserModel, ICmsUser
{
    #region Constant user objects for Unknown/Anonymous

    internal static readonly CmsUserRaw AnonymousUser = new()
    {
        Id = -1,
        Name = SxcUserConstants.Anonymous,
    };

    internal static readonly CmsUserRaw UnknownUser = new()
    {
        Id = -2,
        Name = Eav.Constants.NullNameId,
    };

    #endregion

    IMetadata ICmsUser.Metadata
        => throw new NotSupportedException($"Metadata currently not supported on CmsUser from {nameof(IUserService)}");

    IMetadataOf IHasMetadata.Metadata
        => throw new NotSupportedException($"Metadata currently not supported on CmsUser from {nameof(IUserService)}");

}