using ToSic.Eav.Data.Raw.Sys;

namespace ToSic.Eav.WebApi.Sys.Admin;

internal static class ContentTypeFieldDtoRaw
{
    internal static IRawEntity ToRawEntity(this ContentTypeFieldDto field) => new RawEntity(new()
    {
        { nameof(ContentTypeFieldDto.Id), field.Id },
        { nameof(ContentTypeFieldDto.SortOrder), field.SortOrder },
        { nameof(ContentTypeFieldDto.Type), field.Type },
        { nameof(ContentTypeFieldDto.InputType), field.InputType },
        { nameof(ContentTypeFieldDto.StaticName), field.StaticName },
        { nameof(ContentTypeFieldDto.IsTitle), field.IsTitle },
        { nameof(ContentTypeFieldDto.AttributeId), field.AttributeId },
        { nameof(ContentTypeFieldDto.Metadata), field.Metadata },
        { nameof(ContentTypeFieldDto.InputTypeConfig), field.InputTypeConfig },
        { nameof(ContentTypeFieldDto.Permissions), field.Permissions },
        { nameof(ContentTypeFieldDto.ImageConfiguration), field.ImageConfiguration },
        { nameof(ContentTypeFieldDto.IsEphemeral), field.IsEphemeral },
        { nameof(ContentTypeFieldDto.HasFormulas), field.HasFormulas },
        { nameof(ContentTypeFieldDto.EditInfo), field.EditInfo },
        { nameof(ContentTypeFieldDto.Guid), field.Guid },
        { nameof(ContentTypeFieldDto.SysSettings), field.SysSettings },
        { nameof(ContentTypeFieldDto.ContentType), field.ContentType },
        { nameof(ContentTypeFieldDto.ConfigTypes), field.ConfigTypes },
    }) { Id = field.Id };
}
