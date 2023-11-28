namespace ToSic.Sxc.Oqt.Server.Controllers;

public abstract class OqtStatefulControllerBase : OqtControllerBase
{
    #region Setup

    protected OqtStatefulControllerBase(string logSuffix) : base(true, logSuffix)
    {
    }

    #endregion
}