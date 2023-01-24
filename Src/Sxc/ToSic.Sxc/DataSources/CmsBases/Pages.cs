using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

// Important Info to people working with this
// It's an abstract class, and must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific implementation
// This is because any constructor DI should be able to target this type, and get the real implementation

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Deliver a list of pages from the current platform (Dnn or Oqtane).
    ///
    /// As of now there are no parameters to set.
    /// </summary>
    [PublicApi]
    [VisualQuery(
        ExpectsDataOfType = VqExpectsDataOfType,
        GlobalName = VqGlobalName,
        HelpLink = VqHelpLink,
        Icon = VqIcon,
        NiceName = VqNiceName,
        Type = VqType,
        UiHint = VqUiHint)]
    public abstract class Pages: ExternalData
    {
        #region Public Consts for inheriting implementations

        // ReSharper disable UnusedMember.Global
        [PrivateApi] public const string VqGlobalName = "e35031b2-3e99-41fe-a5ac-b79f447d5800";
        [PrivateApi] public const string VqExpectsDataOfType = "";
        [PrivateApi] public const string VqNiceName = "Pages";
        [PrivateApi] public const string VqUiHint = "Pages in this site";
        [PrivateApi] public const DataSourceType VqType = DataSourceType.Source;
        [PrivateApi] public const string VqIcon = Icons.PageFind;
        [PrivateApi] public const string VqHelpLink = "https://r.2sxc.org/ds-pages";
        // ReSharper restore UnusedMember.Global

        #endregion

        #region Configuration properties - As of now no properties ATM


        #endregion

        #region Constructor

        [PrivateApi]
        protected Pages(Dependencies dependencies): base(dependencies, $"SDS.Pages")
        {
            Provide(GetPages);
        }
        #endregion

        #region Inner Class Just For Processing

        /// <summary>
        /// The inner list retrieving the pages and doing security checks etc. 
        /// </summary>
        /// <returns></returns>
        [PrivateApi]
        protected abstract List<TempPageInfo> GetPagesInternal();

        [PrivateApi]
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

        [PrivateApi]
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
                            {DataConstants.TitleField, p.Title},
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
