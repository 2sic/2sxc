using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.DataSources.CmsBases
{
    [PrivateApi("this is half a DataSource - the final implementation must come from each platform")]
    // additional info so the visual query can provide the correct buttons and infos
    public abstract class RolesDataSourceBase : ExternalData
    {
        #region Public Consts for inheriting implementations

        public const string VqNiceName = "Roles";
        public const string VqIcon = "date_range";
        public const string VqUiHint = "Roles in the CMS";
        public const string VqGlobalName = "eee54266-d7ad-4f5e-9422-2d00c8f93b45"; // random & unique Guid
        public const DataSourceType VqType = DataSourceType.Source;
        public const string VqExpectsDataOfType = "";
        public const string VqHelpLink = ""; // TODO

        #endregion

        #region Configuration-properties
        private const string RolesFilterKey = "RolesFilter";

        /// <summary>
        /// Optional RolesFilter (comma-separated integers) filter,
        /// exclude roles based on roleId
        /// </summary>
        public string RolesFilter
        {
            get => Configuration[RolesFilterKey];
            set => Configuration[RolesFilterKey] = value;
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor to tell the system what out-streams we have
        /// </summary>
        protected RolesDataSourceBase()
        {
            Provide(GetList); // default out, if accessed, will deliver GetList

            ConfigMask(RolesFilterKey, "[Settings:RolesFilter]");

            // TEST cases
            //Configuration[RolesFilterKey] = "1096,1097,1101,1102,1103";
            //Configuration[RolesFilterKey] = "1102,1103";
            //Configuration[RolesFilterKey] = "1101";
            //Configuration[RolesFilterKey] = "not-a-integer,-1";
        }

        #endregion

        public IImmutableList<IEntity> GetList()
        {
            var wrapLog = Log.Fn<IImmutableList<IEntity>>();
            var roles = GetRolesInternal()?.ToList();

            if (roles == null || !roles.Any()) return wrapLog.Return(new ImmutableArray<IEntity>(), "null/empty");

            // This will resolve the tokens before starting
            Configuration.Parse();

            if (!string.IsNullOrEmpty(RolesFilter))
            {
                var filter = RolesFilter.Split(',')
                    .Select(r => int.TryParse(r.Trim(), out var roleId) ? roleId : -1)
                    .Where(r => r != -1).ToList();
                if (filter.Any())
                    roles = roles.Where(r => !filter.Contains(r.Id)).ToList();
            }

            var result = roles
                .Select(r =>
                    DataBuilder.Entity(new Dictionary<string, object>
                        {
                            {"Name", r.Name},
                        },
                        id: r.Id,
                        created: r.Created,
                        modified: r.Modified,
                        titleField: "Name"))
                .ToImmutableList();

            return wrapLog.Return(result, "found");
        }

        #region Inner Class Just For Processing

        /// <summary>
        /// The inner list retrieving the pages and doing security checks etc. 
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<RoleDataSourceInfo> GetRolesInternal();
        
        protected class RoleDataSourceInfo : IRole
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime Created { get; set; }
            public DateTime Modified { get; set; }
        }

        #endregion
    }
}
