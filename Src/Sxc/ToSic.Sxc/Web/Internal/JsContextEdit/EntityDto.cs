namespace ToSic.Sxc.Web.Internal.JsContextEdit;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class EntityDto
{
    public int ZoneId { get; protected set; }  // the zone of the content-block
    public int AppId { get; protected set; }   // the zone of the content-block
    public Guid Guid { get; protected set; }   // the entity-guid of the content-block
    public int Id { get; protected set; }      // the entity-id of the content-block
}