using ToSic.Eav.Apps;
using ToSic.Eav.Documentation;
using ToSic.Eav.Environment;
using ToSic.Sxc.Apps.Blocks;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// A unit / block of output in a CMS. 
    /// </summary>
    [PublicApi]
    public interface IBlock: IInAppAndZone
    {
        [PrivateApi]
        bool ShowTemplateChooser { get; }

        [PrivateApi]
        bool ParentIsEntity { get; }   // alternative is module
        
        [PrivateApi]
        int ParentId { get; }

        [PrivateApi]
        bool DataIsMissing { get; }

        [PrivateApi]
        int ContentBlockId { get; }

        [PrivateApi]
        string ParentFieldName { get; }
        [PrivateApi]
        int ParentFieldSortOrder { get; }

        #region Values related to the current unit of content / the view
        
        ITenant Tenant { get; }

        IView View { get; set; }

        BlockConfiguration Configuration { get; }

        IApp App { get; }

        IBlockDataSource Data { get; }

        bool IsContentApp { get; }
        #endregion

        [PrivateApi]
        ICmsBlock CmsInstance { get; }

        [PrivateApi]
        bool ContentGroupExists { get; }

        [PrivateApi]
        // todo: should get rid of this asap.
        BlockEditorBase Editor { get; }
    }
}
