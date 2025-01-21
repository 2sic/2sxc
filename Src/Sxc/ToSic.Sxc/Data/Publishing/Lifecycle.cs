namespace ToSic.Sxc.Data;

internal class Lifecycle(IEntity entity): ILifecycle
{
    public int Version => entity.Version;
    public DateTime Created => entity.Created;
    public DateTime Modified => entity.Modified;
    public int OwnerId => entity.OwnerId;
}