using ToSic.Eav.Apps;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Apps.Blocks;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks
{
    public interface IBlock: IZoneIdentity
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

        IView View { get; set; }

        BlockConfiguration Configuration { get; }

        IApp App { get; }

        IBlockDataSource Data { get; }

        bool IsContentApp { get; }
        #endregion
        ICmsBlock CmsInstance { get; }

        bool ContentGroupExists { get; }

        [PrivateApi]
        BlockEditorBase Editor { get; }
    }
}
