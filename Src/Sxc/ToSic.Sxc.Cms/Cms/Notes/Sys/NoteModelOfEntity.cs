namespace ToSic.Sxc.Cms.Notes.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
[ModelSpecs(ContentType = ContentTypeNameId)]
internal record NoteModelOfEntity : ModelOfEntity, INoteModel
{
    public const string ContentTypeNameId = "5e958dc6-2922-4d68-835c-7b9711538b12";
    internal const string TypeName = "Note";

    public int Id => Entity.EntityId;
    public Guid Guid => Entity.EntityGuid;
    public DateTime Created => Entity.Created;
    public DateTime Modified => Entity.Modified;

    public string? Note => GetThis<string>(null);

    public string? NoteType => GetThis(fallback: "note");
}