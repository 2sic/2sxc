using ToSic.Eav.CodeChanges;
using ToSic.Sxc.Dnn.Services;
using static ToSic.Eav.CodeChanges.CodeChangeInfo;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Obsolete, use <see cref="ToSic.Sxc.Services.ISystemLogService"/> instead
    /// </summary>
    public class LogServiceUsingOldInterface: DnnSystemLogService
    {
        public LogServiceUsingOldInterface(CodeChangeService codeChanges)
        {
            codeChanges.Warn(V16To18("ToSic.Sxc.Services.ILogService", message: "Use ToSic.Sxc.Services.ISystemLogService instead"));
        }
    }
}
