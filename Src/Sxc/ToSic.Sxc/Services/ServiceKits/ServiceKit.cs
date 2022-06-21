using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// **_BETA_**
    /// 
    /// Root / base class for **ServiceKits**.
    /// ServiceKits are a bundle of services which are quickly available when you need them.
    /// To use them in Razor, you'll do things like:
    ///
    /// `@Kit.Page.Activate("fancybox3")`
    ///
    /// They are
    /// * _lazy_ so they are only created if ever needed
    /// * _created and preserved_ so multiple access to the service won't cause any overhead
    /// * _versioned_ so that we can continue to enhance the APIs without breaking existing Razor
    ///
    /// This is the base/dummy class and doesn't provide any services/properties. 
    /// As of v14.03+ you should use the <see cref="ServiceKit14"/>
    /// </summary>
    /// <remarks>
    /// * History: Added v14.03 WIP
    /// * Everything that needs a ServiceKit will have a "where TKit : <see cref="ServiceKit14"/>"
    /// * It's not abstract, so that you can use it as the placeholder in cases where you don't need a real kit
    /// </remarks>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("BETA / WIP v14.05")]
    public class ServiceKit: INeedsDynamicCodeRoot, IHasDynamicCodeRoot
    {
        /// <inheritdoc />
        [PrivateApi]
        public void ConnectToRoot(IDynamicCodeRoot codeRoot) => _DynCodeRoot = codeRoot;

        /// <inheritdoc />
        [PrivateApi]
        public IDynamicCodeRoot _DynCodeRoot { get; private set; }

        /// <summary>
        /// All the services provided by this kit must come from the code root so they are properly initialized. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [PrivateApi]
        protected T GetService<T>() => _DynCodeRoot.GetService<T>();
    }
}
