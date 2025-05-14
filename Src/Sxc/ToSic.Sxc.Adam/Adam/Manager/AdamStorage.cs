using ToSic.Lib.Services;

namespace ToSic.Sxc.Adam.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class AdamStorage(string logName) : ServiceBase(logName)
{
    /// <summary>
    /// Root of this container
    /// </summary>
    public string Root => GeneratePath("");


    /// <summary>
    /// Figure out the path to a subfolder within this container
    /// </summary>
    /// <param name="subFolder"></param>
    /// <returns></returns>
    protected abstract string GeneratePath(string subFolder);

        
}