using DotNetNuke.Entities.Users;
using ToSic.Sys.Users;
using ToSic.Sys.Users.Permissions;

namespace ToSic.Sxc.Dnn.Context;

internal record DnnUser(UserInfo? DnnUserInfo, EffectivePermissions? EffectivePermissions) : IUser<UserInfo>
{
    public required int Id { get; init; }
    public required Guid Guid { get; init; }
    public required string IdentityToken { get; init; }

    public bool IsAnonymous => Id == -1;
    public bool IsSystemAdmin => DnnUserInfo?.IsSuperUser ?? false;
    public bool IsSiteAdmin => EffectivePermissions?.IsSiteAdmin ?? false;
    public bool IsContentAdmin => EffectivePermissions?.IsContentAdmin ?? false;
    public bool IsContentEditor => EffectivePermissions?.IsContentEditor ?? false;
    public bool IsSiteDeveloper => IsSystemAdmin;

    public string Username => DnnUserInfo?.Username ?? "";

    public string Name => DnnUserInfo?.DisplayName ?? "";

    public string Email => DnnUserInfo?.Email ?? "";
    public required List<int> Roles { get; init; }

    public UserInfo GetContents() => DnnUserInfo;
}