using ToSic.Eav.Apps;
using ToSic.Eav.DataSource.Query.Sys;
using ToSic.Eav.Models;
using ToSic.Sxc.Blocks.Sys.Views;

// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.

namespace ToSic.Sxc.Blocks.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public record BlockConfiguration: ModelOfEntity, IAppIdentity
{
    public  int ZoneId { get; }
    public  int AppId { get; }

    public IEntity? PreviewViewEntity { get; }

    public IBlockIdentifier? BlockIdentifierOrNull { get; set; }

    private readonly Generator<QueryDefinitionFactory> _qDefBuilder;

    public BlockConfiguration(IEntity? entity, IAppIdentity appIdentity, IEntity? previewViewEntity, Generator<QueryDefinitionFactory> qDefBuilder, string languageCode, ILog parentLog):
        base(entity!)
    {
        parentLog.SubLogOrNull("Blk.Config").A("Entity is " + (entity == null ? "" : "not") + " null");
        LookupLanguages = [languageCode];
        _qDefBuilder = qDefBuilder;
        ZoneId = appIdentity.ZoneId;
        AppId = appIdentity.AppId;
        PreviewViewEntity = previewViewEntity;
    }
        
    internal BlockConfiguration WarnIfMissingData() =>
        Entity != null
            ? this
            : throw new("BlockConfiguration entity is null. " +
                        "This usually happens when you are duplicating a site, and have not yet imported the other content/apps. " +
                        "If that is your issue, check 2sxc.org/help?tag=export-import");

    /// <summary>
    /// Returns true if a content group entity for this group really exists
    /// Means for example, that the app can't be changed anymore
    /// </summary>
    public bool Exists => Entity != null!;

    internal bool DataIsMissing = false;


    #region View stuff

    /// <summary>
    /// The view as it is in the configuration.
    /// This can be different from the view which is actually used,
    /// as it could be replaced by a different view because of url-parameter.
    /// </summary>
    public IView? View
    {
        get
        {
            if (field != null)
                return field;

            // if we're previewing another template, look that up
            var viewEntity = PreviewViewEntity
                             ?? Entity
                                 ?.Children(ViewParts.ViewFieldInContentBlock)
                                 .FirstOrDefault();
            return field = viewEntity == null
                ? null
                : new View(viewEntity, LookupLanguages, _qDefBuilder);
        }
    }

    #endregion

    #region Retrieve the lists - either as object or by the type-indexer

    /// <summary>
    /// Content is a bit special, it must always return a list with at least one null-item
    /// </summary>
    public IList<IEntity?> Content
    {
        get
        {
            if (Entity == null!)
                return [null];
            var list = Entity.Children(ViewParts.Content).ToListOpt();
            return list.Count > 0
                ? list
                : [null];
        }
    }

    [field: AllowNull, MaybeNull]
    public IList<IEntity?> Presentation => field ??= Entity?.Children(ViewParts.Presentation).ToListOpt() ?? [];

    public IList<IEntity?> Header => Entity?.Children(ViewParts.FieldHeader).ToListOpt() ?? [];

    public IList<IEntity?> HeaderPresentation => Entity?.Children(ViewParts.ListPresentation).ToListOpt() ?? [];

    public IList<IEntity> this[string type] =>
        type.ToLowerInvariant() switch
        {
            ViewParts.ContentLower => Content,
            ViewParts.PresentationLower => Presentation,
            ViewParts.ListContentLower => Header,
            ViewParts.ListPresentationLower => HeaderPresentation,
            _ => throw new("Type " + type + " not allowed")
        };

    #endregion
}