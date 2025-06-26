namespace ToSic.Sxc.Oqt.Server.Controllers;

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class OqtStatefulControllerBase(string logSuffix) : OqtControllerBase(true, logSuffix);