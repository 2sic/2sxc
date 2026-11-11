using ToSic.Eav.Data.Sys.Entities;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Data.Sys.Decorators;

namespace ToSic.Sxc.Data.Sys.Convert;

/// <summary>
/// Convert various types of entities (standalone, dynamic, in streams, etc.) to Dictionaries <br/>
/// Mainly used for serialization scenarios, like in WebApis.
/// </summary>
/// <remarks>
/// Standard constructor, important for opening this class in dependency-injection
/// </remarks>
[PrivateApi("Hide implementation; this was never public; the DataToDictionary was with empty constructor, but that's already polyfilled")]
[ShowApiWhenReleased(ShowApiMode.Never)]
[method: PrivateApi]
public class ConvertToEavLightWithCmsInfo(ConvertToEavLight.Dependencies services) : ConvertToEavLight(services)
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

        AddPresentation(entity, dictionary!);

        return dictionary;
    }

    #region to enhance serializable IEntities with 2sxc specific infos

    private void AddPresentation(IEntity entity, EavLightEntity dictionary)
    {
        var decorator = entity.GetDecorator<EntityInBlockDecorator>();

        // Add full presentation object if it has one...because there we need more than just id/title
        if (decorator?.Presentation == null || dictionary.ContainsKey(ViewParts.Presentation))
            return;

        // if (entityInGroup.Presentation != null)
        dictionary.Add(ViewParts.Presentation, GetDictionaryFromEntity(decorator.Presentation));
    }


    #endregion
}
