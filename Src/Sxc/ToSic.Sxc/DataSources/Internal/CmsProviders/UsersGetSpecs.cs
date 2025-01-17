using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.DataSources.Internal
{
    public record UsersGetSpecs
    {
        /// <summary>
        /// Optional Users (single value or comma-separated guids or integers) filter,
        /// include users based on guid or id
        /// </summary>
        public string UserIds { get; set; }

        /// <summary>
        /// Optional exclude Users (single value or comma-separated guids or integers) filter,
        /// exclude users based on guid or id
        /// </summary>
        public string ExcludeUserIds { get; set; }

        /// <summary>
        /// Optional IncludeRolesFilter (single value or comma-separated integers) filter,
        /// include users that have any of roles from filter
        /// </summary>
        public string RoleIds { get; set; }

        /// <summary>
        /// Optional ExcludeRolesFilter (single value or comma-separated integers) filter,
        /// exclude users that have any of roles from filter
        /// </summary>
        public string ExcludeRoleIds { get; set; }

        /// <summary>
        /// Optional SystemAdmins filter.
        /// 
        /// * `true` - with System Admins
        /// * `false` - without System Admins
        /// * `required` - only System Admins (no normal users)
        /// </summary>
        public string IncludeSystemAdmins { get; set; }

        /// <summary>
        /// Add property `Roles` as a relationship to role entities.
        /// </summary>
        public bool AddRoles { get; set; }


        #region Configuration

        public IEnumerable<int> UserIdFilter { get; private set; }
        public IEnumerable<Guid> UserGuidFilter { get; private set; }
        public IEnumerable<int> ExcludeUserIdsFilter { get; private set; }
        public IEnumerable<Guid> ExcludeUserGuidsFilter { get; private set; }
        public IEnumerable<int> RolesFilter { get; private set; }
        public IEnumerable<int> ExcludeRolesFilter { get; private set; }

        #endregion

        #region Other Constants
        private const char Separator = ',';
        private const int NullInteger = -1;
        public const string IncludeRequired = "required";
        public const string IncludeOptional = "true";
        public const string IncludeForbidden = "false";
        #endregion

        public UsersGetSpecs Init()
        {
            UserIdFilter = ParseCsvToIntList(UserIds);
            UserGuidFilter = ParseCsvToGuidList(UserIds);
            ExcludeUserIdsFilter = ParseCsvToIntList(ExcludeUserIds);
            ExcludeUserGuidsFilter = ParseCsvToGuidList(ExcludeUserIds);
            RolesFilter = ParseCsvToIntList(RoleIds);
            ExcludeRolesFilter = ParseCsvToIntList(ExcludeRoleIds);
            return this;
        }

        private static List<int> ParseCsvToIntList(string stringList)
            => !stringList.HasValue()
                ? []
                : stringList.Split(Separator)
                    .Select(u => int.TryParse(u.Trim(), out var userId) ? userId : NullInteger)
                    .Where(u => u != -1)
                    .ToList();

        private static List<Guid> ParseCsvToGuidList(string stringList)
            => !stringList.HasValue()
                ? []
                : stringList.Split(Separator)
                    .Select(u => Guid.TryParse(u.Trim(), out var userGuid) ? userGuid : Guid.Empty)
                    .Where(u => u != Guid.Empty)
                    .ToList();
    }
}
