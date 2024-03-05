using ToSic.Eav.DataFormats.EavLight;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Data.Internal.Decorators;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Data.Internal.Convert;

/// <summary>
/// Convert various types of entities (standalone, dynamic, in streams, etc.) to Dictionaries <br/>
/// Mainly used for serialization scenarios, like in WebApis.
/// </summary>
/// <remarks>
/// Standard constructor, important for opening this class in dependency-injection
/// </remarks>
[PrivateApi("Hide implementation; this was never public; the DataToDictionary was with empty constructor, but that's already polyfilled")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
[method: PrivateApi]
public class ConvertToEavLightWithCmsInfo(ConvertToEavLight.MyServices services) : ConvertToEavLight(services)
{
    /// <summary>
    /// Determines if we should use edit-information
    /// </summary>
    [PrivateApi("Note: wasn't private till 2sxc 12.04, very low risk of it being published. Set was always internal")]
    public bool WithEdit { get; set; }

    [PrivateApi]
    protected override EavLightEntity GetDictionaryFromEntity(IEntity entity)
    {
        // Do groundwork
        var dictionary = base.GetDictionaryFromEntity(entity);

        AddPresentation(entity, dictionary);

        // The edit info is an old feature. To phase out, we'll disable it if the new $select is used
        if (!PresetFilters.SerializeTitleForce == true)
            AddEditInfo(entity, dictionary);

        return dictionary;
    }

    #region to enhance serializable IEntities with 2sxc specific infos

    private void AddPresentation(IEntity entity, IDictionary<string, object> dictionary)
    {
        var decorator = entity.GetDecorator<EntityInBlockDecorator>();

        // Add full presentation object if it has one...because there we need more than just id/title
        if (decorator?.Presentation == null || dictionary.ContainsKey(ViewParts.Presentation)) return;

        // if (entityInGroup.Presentation != null)
        dictionary.Add(ViewParts.Presentation, GetDictionaryFromEntity(decorator.Presentation));
    }

    /// <summary>
    /// Add additional information in case we're in edit mode
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="dictionary"></param>
    private void AddEditInfo(IEntity entity, IDictionary<string, object> dictionary)
    {
        if (!WithEdit) return;

        // 2024-02-29 2dm - this is old, and I believe not used any more, commented out
        // At least in the Edit-UI it shouldn't be used, 
        // But the key was still found in the inpage, so we're not sure if we can get rid of it

        var title = entity.GetBestTitle(Languages);
        if (string.IsNullOrEmpty(title))
            title = "(no title)";

        var editDecorator = entity.GetDecorator<EntityInBlockDecorator>();

        dictionary.Add(SxcUiConstants.JsonEntityEditNodeName, editDecorator != null // entity is IHasEditingData entWithEditing
            ? (object)new
            {
                sortOrder = editDecorator.SortOrder,
                isPublished = entity.IsPublished,
            }
            : new
            {
                entityId = entity.EntityId,
                title,
                isPublished = entity.IsPublished,
            });
    }

    #endregion
}