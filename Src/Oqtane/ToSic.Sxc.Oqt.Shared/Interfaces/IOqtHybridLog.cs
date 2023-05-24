namespace ToSic.Sxc.Oqt.Shared.Interfaces;

public interface IOqtHybridLog
{
    /// <summary>
    /// console.log
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    void Log(params object[] message);
}