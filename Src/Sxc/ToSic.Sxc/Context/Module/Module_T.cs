using ToSic.Eav.Apps.Run;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// A base implementation of the block information wrapping the CMS specific object along with it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [PrivateApi("this is just fyi")]
    public abstract class Module<T>: ServiceBase, IModule, IWrapper<T> where T: class
    {
        #region Constructors and DI

        public T GetContents() => UnwrappedModule;
        [PrivateApi] protected T UnwrappedModule;

        protected Module(string logName) : base(logName) { }

        public IModule Init(T item)
        {
            UnwrappedModule = item;
            return this;
        }

        public abstract IModule Init(int id);
        #endregion

        /// <inheritdoc />
        public abstract int Id { get; }

        /// <inheritdoc />
        public abstract bool IsContent { get; }

        /// <inheritdoc />
        public abstract IBlockIdentifier BlockIdentifier { get; }
    }
}
