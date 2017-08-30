using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.SexyContent.ContentBlocks;
using ToSic.SexyContent.DataSources;

namespace ToSic.SexyContent.Interfaces
{
    internal interface IContentBlock
    {
        // bool ContentGroupIsCreated { get;  }
        bool ShowTemplateChooser { get; }

        /// <summary>
        /// Warning: 2017-08-30 2dm I believe this has the worng value (is always false) but this wasn't noticed because it's not in use
        /// </summary>
        bool ParentIsEntity { get; }   // alternative is module
        int ParentId { get; }

        bool DataIsMissing { get; }

        int ContentBlockId { get; }
        string ParentFieldName { get; }
        int ParentFieldSortOrder { get; }

        #region Values related to the current unit of content / the view
        int AppId { get; }
        int ZoneId { get; }
        PortalSettings PortalSettings { get; }

        Template Template { get; set; }

        ContentGroup ContentGroup { get; }

        App App { get; }

        ViewDataSource Data { get; }

        bool IsContentApp { get; }
        #endregion
        SxcInstance SxcInstance { get; }

        bool ContentGroupExists { get; }

        ContentGroupReferenceManagerBase Manager { get; }
    }
}
