namespace ToSic.Sxc.Oqt.Server.Controllers;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class OqtStatefulControllerBase : OqtControllerBase
{
    #region Setup

    protected OqtStatefulControllerBase(string logSuffix) : base(true, logSuffix)
    {
    }

    #endregion
}