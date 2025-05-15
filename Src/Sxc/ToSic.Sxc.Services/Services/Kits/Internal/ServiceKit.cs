using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Services;

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
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ServiceKit(string logName) : ServiceForDynamicCode(logName)
{

    /// <summary>
    /// All the services provided by this kit must come from the code root, so they are properly initialized.
    ///
    /// Will first try to use the GetService method to ensure that changes in the Kit (like 16/14) still return
    /// the identical sub-services.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    [PrivateApi]
    protected TService GetKitService<TService>() where TService : class
        => ExCtx.GetService<TService>(reuse: true);
}