using ToSic.Eav.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Code
{
    [PrivateApi]
    public interface IHasCodeLog
    {
        ICodeLog Log { get; }

        //[PrivateApi] ILog Log15 { get; }
    }
}
