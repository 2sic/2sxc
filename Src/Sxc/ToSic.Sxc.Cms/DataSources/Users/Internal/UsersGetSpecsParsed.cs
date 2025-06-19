using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.Cms.Users.Internal;

namespace ToSic.Sxc.DataSources.Internal;

public record UsersGetSpecsParsed(UsersGetSpecs Specs)
{
    #region Configuration

    [field: AllowNull, MaybeNull]
    public IEnumerable<int> UserIdFilter => field ??= ParseCsvToIntList(Specs.UserIds);

    [field: AllowNull, MaybeNull]
    public IEnumerable<Guid> UserGuidFilter => field ??= ParseCsvToGuidList(Specs.UserIds);

    [field: AllowNull, MaybeNull]
    public IEnumerable<int> ExcludeUserIdsFilter => field ??= ParseCsvToIntList(Specs.ExcludeUserIds);

    [field: AllowNull, MaybeNull]
    public IEnumerable<Guid> ExcludeUserGuidsFilter => field ??= ParseCsvToGuidList(Specs.ExcludeUserIds);

    [field: AllowNull, MaybeNull]
    public IEnumerable<int> RolesFilter => field ??= ParseCsvToIntList(Specs.RoleIds);

    [field: AllowNull, MaybeNull]
    public IEnumerable<int> ExcludeRolesFilter => field ??= ParseCsvToIntList(Specs.ExcludeRoleIds);

    #endregion

    private static List<int> ParseCsvToIntList(string? stringList)
        => !stringList.HasValue()
            ? []
            : stringList.Split(UserConstants.Separator)
                .Select(u => int.TryParse(u.Trim(), out var userId) ? userId : UserConstants.NullInteger)
                .Where(u => u != -1)
                .ToList();

    private static List<Guid> ParseCsvToGuidList(string? stringList)
        => !stringList.HasValue()
            ? []
            : stringList.Split(UserConstants.Separator)
                .Select(u => Guid.TryParse(u.Trim(), out var userGuid) ? userGuid : Guid.Empty)
                .Where(u => u != Guid.Empty)
                .ToList();

}