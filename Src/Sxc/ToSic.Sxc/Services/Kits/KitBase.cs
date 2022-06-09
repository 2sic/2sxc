using ToSic.Sxc.Code;

namespace ToSic.Sxc.Services.Kits
{
    public abstract class KitBase: INeedsDynamicCodeRoot, IHasDynamicCodeRoot
    {
        public void ConnectToRoot(IDynamicCodeRoot codeRoot) => _DynCodeRoot = codeRoot;

        public IDynamicCodeRoot _DynCodeRoot { get; private set; }

        /// <summary>
        /// All the services provided by this kit must come from the code root so they are properly initialized. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetService<T>() => _DynCodeRoot.GetService<T>();

    }
}
