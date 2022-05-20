using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.DataSources.CmsBases
{
    [PrivateApi("this is half a DataSource - the final implementation must come from each platform")]
    public abstract class PagesBase: ExternalData
    {
        #region Public Consts for inheriting implementations

        // ReSharper disable UnusedMember.Global
        public const string VqGlobalName = "e35031b2-3e99-41fe-a5ac-b79f447d5800";
        public const string VqExpectsDataOfType = "";
        public const string VqNiceName = "Pages";
        public const string VqUiHint = "Pages in the CMS";
        public const DataSourceType VqType = DataSourceType.Source;
        public const string VqIcon = "find_in_page";
        public const string VqHelpLink = ""; // todo
        // ReSharper restore UnusedMember.Global

        #endregion

        #region Configuration properties - As of now no properties ATM


        //public int Root { get; set; } = 0;


        #endregion

        #region Constructor


        protected PagesBase()
        {
            Provide(GetPages);
        }
        #endregion

        #region Inner Class Just For Processing

        /// <summary>
        /// The inner list retrieving the pages and doing security checks etc. 
        /// </summary>
        /// <returns></returns>
        protected abstract List<TempPageInfo> GetPagesInternal();
        
        protected class TempPageInfo
        {
            public int Id;
            public int ParentId;
            public Guid? Guid;
            public string Title;
            public string Name;
            public bool Visible;
            public string Path;
            public string Url;
            public DateTime Created;
            public DateTime Modified;
        }

        #endregion

        public IImmutableList<IEntity> GetPages()
        {
            var wrapLog = Log.Fn<IImmutableList<IEntity>>();
            var pages = GetPagesInternal();

            if (pages == null || !pages.Any()) return wrapLog.Return(new ImmutableArray<IEntity>(), "null/empty");

            var result = pages
                .Select(p =>
                    DataBuilder.Entity(new Dictionary<string, object>
                        {
                            //{"Id", p.Id},
                            {"Title", p.Title},
                            {"Name", p.Name},
                            {"ParentId", p.ParentId},
                            //{"Guid", p.Guid},
                            {"Visible", p.Visible},
                            {"Path", p.Path},
                            {"Url", p.Url},
                        },
                        id: p.Id,
                        guid: p.Guid,
                        created: p.Created,
                        modified: p.Modified,
                        titleField: "Name"))
                .ToImmutableList();

            return wrapLog.Return(result, "found");
        }
    }
}
