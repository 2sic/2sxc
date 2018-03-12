using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.ValueProvider;
using ToSic.SexyContent.ContentBlocks;
using ToSic.SexyContent.DataSources;

namespace ToSic.SexyContent.Interfaces
{
    internal interface IContentBlock
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
        int ZoneId { get; }

        ITenant Tenant { get; }

        Template Template { get; set; }

        ContentGroup ContentGroup { get; }

        App App { get; }

        ValueCollectionProvider Configuration { get; }

        ViewDataSource Data { get; }

        bool IsContentApp { get; }
        #endregion
        SxcInstance SxcInstance { get; }

        bool ContentGroupExists { get; }

        ContentGroupReferenceManagerBase Manager { get; }
    }
}
