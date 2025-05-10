namespace ToSic.Sxc.Backend.SaveHelpers;

/// <summary>
/// All save helpers usually need the sxc-instance and the log
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class SaveHelperBase(string logName, object[] connect = default) : ServiceBase(logName, connect: connect ?? [])
{
    internal IContextOfApp Context { get; private set; }

    public void Init(IContextOfApp context)
    {
        Context = context;
    }
}