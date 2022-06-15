using ToSic.Sxc.Code;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Root / base class for Kits
    /// Everything that needs a kit will have a "where TKit : Kit"
    /// It's not abstract, so that you can use it as the placeholder in cases where you don't need a real kit
    /// </summary>
    public class Kit: INeedsDynamicCodeRoot, IHasDynamicCodeRoot
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
