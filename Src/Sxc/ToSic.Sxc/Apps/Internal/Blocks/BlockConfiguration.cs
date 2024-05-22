using ToSic.Eav.Apps;
using ToSic.Eav.Cms.Internal;
using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Lib.DI;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Apps.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class BlockConfiguration: EntityBasedWithLog, IAppIdentity
{
    public  int ZoneId { get; }
    public  int AppId { get; }

    internal IEntity PreviewTemplate { get; set; }

    internal IBlockIdentifier BlockIdentifierOrNull;

    private readonly Generator<QueryDefinitionBuilder> _qDefBuilder;

    public BlockConfiguration(IEntity entity, IAppIdentity cmsRuntime, IEntity previewTemplate, Generator<QueryDefinitionBuilder> qDefBuilder, string languageCode, ILog parentLog): base(entity, languageCode, parentLog, "Blk.Config")
    {
        Log.A("Entity is " + (entity == null ? "" : "not") + " null");
        _qDefBuilder = qDefBuilder;
        ZoneId = cmsRuntime.ZoneId;
        AppId = cmsRuntime.AppId;
        PreviewTemplate = previewTemplate;
    }
        
    internal BlockConfiguration WarnIfMissingData()
    {
        if (Entity != null) return this;
        throw new("BlockConfiguration entity is null. " +
                  "This usually happens when you are duplicating a site, and have not yet imported the other content/apps. " +
                  "If that is your issue, check 2sxc.org/help?tag=export-import");
    }
        
    /// <summary>
    /// Returns true if a content group entity for this group really exists
    /// Means for example, that the app can't be changed anymore
    /// </summary>
    public bool Exists => Entity != null;

    internal bool DataIsMissing = false;
        
        
    #region View stuff
        
    public IView View
    {
        get
        {
            if (_view != null) return _view;

            // if we're previewing another template, look that up
            var templateEntity = PreviewTemplate ?? Entity?.Children(ViewParts.ViewFieldInContentBlock).FirstOrDefault();
            return _view = templateEntity == null ? null : new View(templateEntity, LookupLanguages, Log, _qDefBuilder);
        }
    }
    private IView _view;
        
    #endregion

    #region Retrieve the lists - either as object or by the type-indexer

    /// <summary>
    /// Content is a bit special, it must always return a list with at least one null-item
    /// </summary>
    public List<IEntity> Content
    {
        get
        {
            if (Entity == null) return [null];
            var list = Entity.Children(ViewParts.Content);
            return list.Count > 0 ? list : [null];
        }
    }

    public List<IEntity> Presentation => Entity?.Children(ViewParts.Presentation) ?? [];

    public List<IEntity> Header => Entity?.Children(ViewParts.FieldHeader) ?? [];

    public List<IEntity> HeaderPresentation => Entity?.Children(ViewParts.ListPresentation) ?? [];

    public List<IEntity> this[string type]
    {
        get
        {
            switch (type.ToLowerInvariant())
            {
                case ViewParts.ContentLower: return Content;
                case ViewParts.PresentationLower: return Presentation;
                case ViewParts.ListContentLower: return Header;
                case ViewParts.ListPresentationLower: return HeaderPresentation;
                default: throw new("Type " + type + " not allowed");
            }
        }
    }

    #endregion
}