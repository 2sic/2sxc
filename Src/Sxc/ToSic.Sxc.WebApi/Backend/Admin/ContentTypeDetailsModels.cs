//using ToSic.Eav.Data.Raw.Sys;
//using ToSic.Eav.Data.ContentTypes;
//using ToSic.Eav.Data.Raw;

//namespace ToSic.Eav.WebApi.Sys.Admin;

//// TODO: @2rb
//// - Merge this with ContentTypeDto, doesn't make sense to have 2 identical objects
//// - the final object should be ContentTypeDetailsRaw
//// - Should probably just implement IRawModelAutoConvert, not inherit from RawEntity

//[ContentType(
//    Name = "ContentTypeSpecs", // "ContentType", 2026-08-05 2dm, renaming this, as it's not a real content type
//    Guid = "9434ef61-5d89-4abf-9d47-d4093a37ed6f",
//    Description = "Content type details",
//    Scope = "System"
//)]
//public record ContentTypeSpecsRaw(ContentTypeDto contentType) : RawEntity
//{
//    [ContentTypeField(IsTitle = true)] // TODO: REMEMBER TO KEEP
//    public string Name => contentType.Name;

//    protected override IDictionary<string, object?> GetValues() => new Dictionary<string, object?>
//    {
//        { nameof(ContentTypeDto.Id), contentType.Id },
//        { nameof(ContentTypeDto.Name), contentType.Name },
//        { nameof(ContentTypeDto.Label), contentType.Label },
//        { nameof(ContentTypeDto.StaticName), contentType.StaticName },
//        { nameof(ContentTypeDto.NameId), contentType.NameId },
//        { nameof(ContentTypeDto.Scope), contentType.Scope },
//        { nameof(ContentTypeDto.Description), contentType.Description },
//        { nameof(ContentTypeDto.UsesSharedDef), contentType.UsesSharedDef },
//        { nameof(ContentTypeDto.SharedDefId), contentType.SharedDefId },
//        { nameof(ContentTypeDto.Items), contentType.Items },
//        { nameof(ContentTypeDto.Fields), contentType.Fields },
//        { nameof(ContentTypeDto.TitleField), contentType.TitleField },
//        { nameof(ContentTypeDto.Metadata), contentType.Metadata },
//        { nameof(ContentTypeDto.Properties), contentType.Properties },
//        { nameof(ContentTypeDto.Permissions), contentType.Permissions },
//        { nameof(ContentTypeDto.EditInfo), contentType.EditInfo },
//    };
//}

//// TODO: @2rb
//// - Merge this with ContentTypeFieldDto, doesn't make sense to have 2 identical objects
//// - the final object should be ContentTypeFieldRaw
//// - Should probably just implement IRawModelAutoConvert, not inherit from RawEntity

//[ContentType(
//    Name = "ContentTypeField",
//    Guid = "eb891b1c-8505-465f-bc51-461cb47ed9c1",
//    Description = "Content type field details",
//    Scope = "System"
//)]
//public record ContentTypeFieldModel(ContentTypeFieldSpecsRaw contentTypeField) : RawEntity
//{
//    [ContentTypeField(IsTitle = true)]  // todo
//    public string Name => contentTypeField.StaticName;

//    protected override IDictionary<string, object?> GetValues() => new Dictionary<string, object?>
//    {
//        { nameof(ContentTypeFieldSpecsRaw.Id), contentTypeField.Id },
//        { nameof(ContentTypeFieldSpecsRaw.SortOrder), contentTypeField.SortOrder },
//        { nameof(ContentTypeFieldSpecsRaw.Type), contentTypeField.Type },
//        { nameof(ContentTypeFieldSpecsRaw.InputType), contentTypeField.InputType },
//        { nameof(ContentTypeFieldSpecsRaw.StaticName), contentTypeField.StaticName },
//        { nameof(ContentTypeFieldSpecsRaw.IsTitle), contentTypeField.IsTitle },
//        { nameof(ContentTypeFieldSpecsRaw.AttributeId), contentTypeField.AttributeId },
//        { nameof(ContentTypeFieldSpecsRaw.Metadata), contentTypeField.Metadata },
//        { nameof(ContentTypeFieldSpecsRaw.InputTypeConfig), contentTypeField.InputTypeConfig },
//        { nameof(ContentTypeFieldSpecsRaw.Permissions), contentTypeField.Permissions },
//        { nameof(ContentTypeFieldSpecsRaw.ImageConfiguration), contentTypeField.ImageConfiguration },
//        { nameof(ContentTypeFieldSpecsRaw.IsEphemeral), contentTypeField.IsEphemeral },
//        { nameof(ContentTypeFieldSpecsRaw.HasFormulas), contentTypeField.HasFormulas },
//        { nameof(ContentTypeFieldSpecsRaw.EditInfo), contentTypeField.EditInfo },
//        { nameof(ContentTypeFieldSpecsRaw.Guid), contentTypeField.Guid },
//        { nameof(ContentTypeFieldSpecsRaw.SysSettings), contentTypeField.SysSettings },
//        { nameof(ContentTypeFieldSpecsRaw.ContentType), contentTypeField.ContentType },
//        { nameof(ContentTypeFieldSpecsRaw.ConfigTypes), contentTypeField.ConfigTypes },
//    };
//}
