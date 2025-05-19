namespace ToSic.Sxc.Adam.Work.Internal;

public record AdamWorkOptions(/*int AppId, string ContentType, Guid ItemGuid, string Field, bool UsePortalRoot*/)
{
    public /*required*/ bool UsePortalRoot { get; init; }// = UsePortalRoot;
    public /*required*/ string Field { get; init; } = "";// = Field;
    public /*required*/ Guid ItemGuid { get; init; }// = ItemGuid;
    public /*required*/ string ContentType { get; init; } = "";// = ContentType;
    public /*required*/ int AppId { get; init; }// = AppId;
}