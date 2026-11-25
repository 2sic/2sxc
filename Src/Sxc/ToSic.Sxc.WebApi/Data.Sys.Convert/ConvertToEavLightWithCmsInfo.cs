using ToSic.Eav.Data.Sys.Entities;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.DataFormats.EavLight.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Data.Sys.Decorators;

namespace ToSic.Sxc.Data.Sys.Convert;

/// <summary>
/// Enhances the standard EavLight converter with the presentation info.
/// Previously also added edit info, but that is probably obsolete now; or handled by entity-specific conversion rules.
/// </summary>
/// <remarks>
/// Standard constructor, important for opening this class in dependency-injection
/// </remarks>
[PrivateApi("Hide implementation; this was never public; the DataToDictionary was with empty constructor, but that's already polyfilled")]
[ShowApiWhenReleased(ShowApiMode.Never)]
[method: PrivateApi]
internal class ConvertToEavLightWithCmsInfo(): IConvertEntityToEavLightExtraProcessor
//internal class ConvertToEavLightWithCmsInfo(ConvertToEavLight.Dependencies services, ConvertEntityToEavLight convShared): ConvertToEavLight(services, convShared)
{
    // 2026-11-11 2dm - dropped this, believe it's not used anymore; keep for reference till ca. 2027-07 #DropWithEdit
    ///// <summary>
    ///// Determines if we should use edit-information
    ///// </summary>
    //[PrivateApi("Note: wasn't private till 2sxc 12.04, very low risk of it being published. Set was always internal")]
    //public bool WithEdit { get; set; }

    //[PrivateApi]
    //protected override EavLightEntity GetDictionaryFromEntity(IEntity entity)
    //{
    //    // Do groundwork
    //    var dictionary = base.GetDictionaryFromEntity(entity);

    //    AddPresentation(entity, dictionary!);

    //    return dictionary;
    //}

    #region to enhance serializable IEntities with 2sxc specific infos

    public EavLightEntity Process(ConvertEntityToEavLight converter, IEntity entity, EavLightEntity eavLightEntity)
    {
        var decorator = entity.GetDecorator<EntityInBlockDecorator>();

        // Add full presentation object if it has one...because there we need more than just id/title
        if (decorator?.Presentation == null || eavLightEntity.ContainsKey(ViewParts.Presentation))
            return eavLightEntity;

        // if (entityInGroup.Presentation != null)
        eavLightEntity.Add(ViewParts.Presentation, converter.GetDictionaryFromEntity(decorator.Presentation));
        return eavLightEntity;
    }


    #endregion

}
