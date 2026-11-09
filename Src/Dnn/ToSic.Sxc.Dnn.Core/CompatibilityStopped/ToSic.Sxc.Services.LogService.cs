using ToSic.Sxc.Dnn.Services;
using ToSic.Sys.Code.InfoSystem;
using DotNetNuke.Abstractions.Logging;

#pragma warning disable IDE0130
namespace ToSic.Sxc.Services;
#pragma warning restore IDE0130

/// <summary>
/// Obsolete, use <see cref="ISystemLogService"/> instead
/// </summary>
internal class LogServiceUsingOldInterface: DnnSystemLogService
{
    public LogServiceUsingOldInterface(CodeInfoService codeInfos, IEventLogger eventLogger) : base(eventLogger)
    {
        throw new("The ToSic.Sxc.Services.ILogServices is deprecated / removed. Please use ToSic.Sxc.Services.ISystemLogService instead.");
        //codeInfos.Warn(V16To18("ToSic.Sxc.Services.ILogService", message: "Use ToSic.Sxc.Services.ISystemLogService instead"));
    }
}
