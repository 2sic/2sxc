using ToSic.Eav.Apps;
using ToSic.SexyContent;
using ToSic.SexyContent.ContentBlocks;
using ToSic.SexyContent.DataSources;
using App = ToSic.SexyContent.App;

namespace ToSic.Sxc.Interfaces
{
    internal interface IContentBlock: IZoneIdentity
    {
        bool ShowTemplateChooser { get; }


        bool ParentIsEntity { get; }   // alternative is module
        int ParentId { get; }

        bool DataIsMissing { get; }

        int ContentBlockId { get; }
        string ParentFieldName { get; }
        int ParentFieldSortOrder { get; }

        #region Values related to the current unit of content / the view
        int AppId { get; }

        ITenant Tenant { get; }

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
