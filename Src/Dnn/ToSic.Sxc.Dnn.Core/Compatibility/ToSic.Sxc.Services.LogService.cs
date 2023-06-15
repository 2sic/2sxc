using ToSic.Eav.Obsolete;
using ToSic.Sxc.Dnn.Services;

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
            codeChanges.Warn(CodeChangeInfo.V13To17($"ToSic.Sxc.Services.{nameof(ILogService)}", message: "Use ToSic.Sxc.Services.ISystemLogService instead"));
        }
    }
}
