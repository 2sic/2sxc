using ToSic.Sxc.Code;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Root / base class for **ServiceKits**.
    /// ServiceKits are a bundle of services which are quickly available when you need them. 
    ///
    /// They are
    /// * _lazy_ so they are only created if ever needed
    /// * _created and preserved_ so multiple access to the service won't cause any overhead
    /// </summary>
    /// <remarks>
    /// * History: Added v14.03 WIP
    /// * Everything that needs a ServiceKit will have a "where TKit : ServiceKitKit"
    /// * It's not abstract, so that you can use it as the placeholder in cases where you don't need a real kit
    /// </remarks>
    public class ServiceKit: INeedsDynamicCodeRoot, IHasDynamicCodeRoot
    {
        /// <inheritdoc />
        public void ConnectToRoot(IDynamicCodeRoot codeRoot) => _DynCodeRoot = codeRoot;

        /// <inheritdoc />
        public IDynamicCodeRoot _DynCodeRoot { get; private set; }

        /// <summary>
        /// All the services provided by this kit must come from the code root so they are properly initialized. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetService<T>() => _DynCodeRoot.GetService<T>();
    }
}
