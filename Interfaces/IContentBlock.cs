using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetNuke.Entities.Portals;
using ToSic.SexyContent.DataSources;

namespace ToSic.SexyContent.Interfaces
{
    internal interface IContentBlock
    {
        // bool ContentGroupIsCreated { get;  }
        bool ShowTemplateChooser { get; }

        bool ParentIsEntity { get; }   // alternative is module
        int ParentId { get; }
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
    }
}
