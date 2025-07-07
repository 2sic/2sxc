﻿using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Data.Sys.Publishing;

// this can just be internal, it's only ever used as an interface
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class Publishing(ITypedItem currentItem, ICodeDataFactory cdf)
    : HelperBase(cdf.Log, "Pub"), IPublishing
{
    // Always supported on IEntity
    public bool IsSupported => true;

    // Entity knows if it's published
    private bool IsPublished => _isPublished ??= currentItem.Entity.IsPublished;
    private bool? _isPublished;

    // Either current is published, or the draft has a different RepositoryId
    public bool HasPublished => IsPublished || currentItem.Entity.RepositoryId != currentItem.Entity.EntityId;

    // Either current is not published, or we found a draft
    public bool HasUnpublished => !IsPublished || UnpublishedEntity != null;

    // Combination of both must be true
    public bool HasBoth => HasUnpublished && HasPublished;

    // Get published - either current, or from appState
    public ITypedItem? GetPublished() => _published.Get(() =>
    {
        if (IsPublished)
            return currentItem;
        var pubEntity = cdf.GetPublished(currentItem.Entity);
        return cdf.AsItem(pubEntity, new() { ItemIsStrict = true });
    });
    private readonly GetOnce<ITypedItem?> _published = new();

    // Get draft - either current, or from appState
    public ITypedItem? GetUnpublished() => _draft.Get(() => !IsPublished
        ? currentItem
        : cdf.AsItem(UnpublishedEntity, new() { ItemIsStrict = true })
    );
    private readonly GetOnce<ITypedItem?> _draft = new();

    /// <summary>
    /// Get draft entity - either current, or from appState.
    /// Do this as a separate step, as we sometimes need the info without converting it to a typed item
    /// </summary>
    private IEntity? UnpublishedEntity => _unPubEntity.Get(() =>
    {
        if (!IsPublished) return currentItem.Entity;
        var draftEntity = cdf.GetDraft(currentItem.Entity);
        return draftEntity;
    });
    private readonly GetOnce<IEntity?> _unPubEntity = new();

    // Get opposite - either draft or published
    public ITypedItem? GetOpposite() => _other.Get(() => IsPublished ? GetUnpublished() : GetPublished());
    private readonly GetOnce<ITypedItem?> _other = new();
}