#pragma warning disable IDE0130
namespace ToSic.Sxc.Services;
#pragma warning restore IDE0130

/// <summary>
/// Obsolete, use <see cref="ISystemLogService"/> instead.
/// Note: 2024-04-22 2dm - believe this could be internal, but not 100% sure
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ILogService: ISystemLogService;