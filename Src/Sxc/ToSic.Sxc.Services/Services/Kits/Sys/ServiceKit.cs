namespace ToSic.Sxc.Services.Sys;

/// <summary>
/// Root / base class for **ServiceKits**.
/// ServiceKits are a bundle of services which are quickly available when you need them.
/// </summary>
/// <remarks>
/// * History: Added v14.04
/// * Everything that needs a ServiceKit will have a "where TKit : <see cref="ServiceKit14"/>"
/// * It's not abstract, so that you can use it as the placeholder in cases where you don't need a real kit (like in DynamicCodeRoot generic types)
/// </remarks>
[PrivateApi("Hidden in v17.02, previously public, but no good reason for it.")]
// #NoEditorBrowsableBecauseOfInheritance
// 2025-05-11 re-enabled, since it shouldn't matter anymore with the new setup
// 2026-09-18 2dm re-disabled
//[ShowApiWhenReleased(ShowApiMode.Never)]
public class ServiceKit(string logName = "Sxc.Kit") : ServiceBase(logName);