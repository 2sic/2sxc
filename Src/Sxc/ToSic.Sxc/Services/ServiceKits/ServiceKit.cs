using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Root / base class for **ServiceKits**.
    /// ServiceKits are a bundle of services which are quickly available when you need them.
    /// </summary>
    /// <remarks>
    /// * History: Added v14.04
    /// * Everything that needs a ServiceKit will have a "where TKit : <see cref="ServiceKit14"/>"
    /// * It's not abstract, so that you can use it as the placeholder in cases where you don't need a real kit
    /// </remarks>
    [PublicApi]
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
