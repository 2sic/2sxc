using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.Data.ContentTypes;

namespace ToSic.Eav.WebApi.Sys.Admin;

// TODO: @2rb
// - Merge this with ContentTypeDto, doesn't make sense to have 2 identical objects
// - the final object should be ContentTypeDetailsRaw
// - Should probably just implement IRawModelAutoConvert, not inherit from RawEntity

[ContentType(
    Name = "ContentType",
    Guid = "c56b9706-c06a-4d48-a362-d0e0036733d4",
    Description = "Content type details",
    Scope = "System"
)]
public record ContentTypeDetailsModel(ContentTypeDto contentType) : RawEntity
{
    [ContentTypeField(IsTitle = true)] // TODO: REMEMBER TO KEEP
    public string Name => contentType.Name;

    protected override IDictionary<string, object?> GetValues() => new Dictionary<string, object?>
    {
        { nameof(ContentTypeDto.Id), contentType.Id },
        { nameof(ContentTypeDto.Name), contentType.Name },
        { nameof(ContentTypeDto.Label), contentType.Label },
        { nameof(ContentTypeDto.StaticName), contentType.StaticName },
        { nameof(ContentTypeDto.NameId), contentType.NameId },
        { nameof(ContentTypeDto.Scope), contentType.Scope },
        { nameof(ContentTypeDto.Description), contentType.Description },
        { nameof(ContentTypeDto.UsesSharedDef), contentType.UsesSharedDef },
        { nameof(ContentTypeDto.SharedDefId), contentType.SharedDefId },
        { nameof(ContentTypeDto.Items), contentType.Items },
        { nameof(ContentTypeDto.Fields), contentType.Fields },
        { nameof(ContentTypeDto.TitleField), contentType.TitleField },
        { nameof(ContentTypeDto.Metadata), contentType.Metadata },
        { nameof(ContentTypeDto.Properties), contentType.Properties },
        { nameof(ContentTypeDto.Permissions), contentType.Permissions },
        { nameof(ContentTypeDto.EditInfo), contentType.EditInfo },
    };
}

// TODO: @2rb
// - Merge this with ContentTypeFieldDto, doesn't make sense to have 2 identical objects
// - the final object should be ContentTypeFieldRaw
// - Should probably just implement IRawModelAutoConvert, not inherit from RawEntity

[ContentType(
    Name = "ContentTypeField",
    Guid = "eb891b1c-8505-465f-bc51-461cb47ed9c1",
    Description = "Content type field details",
    Scope = "System"
)]
public record ContentTypeFieldModel(ContentTypeFieldDto contentTypeField) : RawEntity
{
    [ContentTypeField(IsTitle = true)]  // todo
    public string Name => contentTypeField.StaticName;

    protected override IDictionary<string, object?> GetValues() => new Dictionary<string, object?>
    {
        { nameof(ContentTypeFieldDto.Id), contentTypeField.Id },
        { nameof(ContentTypeFieldDto.SortOrder), contentTypeField.SortOrder },
        { nameof(ContentTypeFieldDto.Type), contentTypeField.Type },
        { nameof(ContentTypeFieldDto.InputType), contentTypeField.InputType },
        { nameof(ContentTypeFieldDto.StaticName), contentTypeField.StaticName },
        { nameof(ContentTypeFieldDto.IsTitle), contentTypeField.IsTitle },
        { nameof(ContentTypeFieldDto.AttributeId), contentTypeField.AttributeId },
        { nameof(ContentTypeFieldDto.Metadata), contentTypeField.Metadata },
        { nameof(ContentTypeFieldDto.InputTypeConfig), contentTypeField.InputTypeConfig },
        { nameof(ContentTypeFieldDto.Permissions), contentTypeField.Permissions },
        { nameof(ContentTypeFieldDto.ImageConfiguration), contentTypeField.ImageConfiguration },
        { nameof(ContentTypeFieldDto.IsEphemeral), contentTypeField.IsEphemeral },
        { nameof(ContentTypeFieldDto.HasFormulas), contentTypeField.HasFormulas },
        { nameof(ContentTypeFieldDto.EditInfo), contentTypeField.EditInfo },
        { nameof(ContentTypeFieldDto.Guid), contentTypeField.Guid },
        { nameof(ContentTypeFieldDto.SysSettings), contentTypeField.SysSettings },
        { nameof(ContentTypeFieldDto.ContentType), contentTypeField.ContentType },
        { nameof(ContentTypeFieldDto.ConfigTypes), contentTypeField.ConfigTypes },
    };
}
