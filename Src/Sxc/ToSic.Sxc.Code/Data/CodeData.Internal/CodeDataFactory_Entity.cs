using ToSic.Eav.Apps;
using ToSic.Sxc.Data.Sys.Decorators;
using ToSic.Sxc.Data.Sys.Typed;

namespace ToSic.Sxc.Data.Internal;

partial class CodeDataFactory
{
    public IEntity AsEntity(object thingToConvert)
        => thingToConvert == null
            ? throw new ArgumentNullException(nameof(thingToConvert))
            : thingToConvert as IEntity
              ?? (thingToConvert as ICanBeEntity)?.Entity
              ?? throw new ArgumentException($"Tried to convert an object to {nameof(IEntity)} but cannot convert a {thingToConvert.GetType()}");

    public IEntity FakeEntity(int appId) => dataBuilderLazy.Value.FakeEntity(appId);

    public IEntity PlaceHolderInBlock(int? appIdOrNull, IEntity? parent, string? fieldName)
    {
        var dummyEntity = FakeEntity(appIdOrNull ?? parent?.AppId ?? 0);
        return parent == null
            ? dummyEntity
            : EntityInBlockDecorator.Wrap(entity: dummyEntity, fieldName: fieldName, parent: parent);
    }

    /// <summary>
    /// Creates an empty list of a specific type, with hidden information to remember what field this is etc.
    /// </summary>
    /// <typeparam name="TTypedItem"></typeparam>
    /// <param name="parent"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    public IEnumerable<TTypedItem> CreateEmptyChildList<TTypedItem>(IEntity parent, string field) where TTypedItem : class, ITypedItem
    {
        // Generate a marker/placeholder to remember what field this is etc.
        var fakeEntity = PlaceHolderInBlock(parent.AppId, parent, field);
        return new ListTypedItems<TTypedItem>(new List<TTypedItem>(), fakeEntity);
    }


    #region Publishing
    private IAppReader AppReader => field ??= ExCtx.GetState<IAppReader>();

    public IEntity? GetDraft(IEntity entity) => AppReader.GetDraft(entity);

    public IEntity? GetPublished(IEntity entity) => AppReader.GetPublished(entity);

    #endregion
}