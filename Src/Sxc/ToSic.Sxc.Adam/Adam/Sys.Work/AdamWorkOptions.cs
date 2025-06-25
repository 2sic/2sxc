namespace ToSic.Sxc.Adam.Sys.Work;

public record AdamWorkOptions
{
    public /*required*/ bool UsePortalRoot { get; init; }// = UsePortalRoot;
    public /*required*/ string Field { get; init; } = "";// = Field;
    public /*required*/ Guid ItemGuid { get; init; }// = ItemGuid;
    public /*required*/ string ContentType { get; init; } = "";// = ContentType;
    public /*required*/ int AppId { get; init; }// = AppId;
}