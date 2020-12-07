using ToSic.Eav.Apps.Run;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// A base implementation of the block information wrapping the CMS specific object along with it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [PrivateApi("this is just fyi")]
    public abstract class Module<T>: HasLog, IModule, IWrapper<T> where T: class
    {
        #region Constructors and DI

        /// <inheritdoc />
        public T UnwrappedContents { get; private set; }

        protected Module(string logName) : base(logName) { }

        public IModule Init(T item, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            UnwrappedContents = item;
            return this;
        }

        public abstract IModule Init(int id, ILog parentLog);
        #endregion

        /// <inheritdoc />
        public abstract int Id { get; /*protected set;*/ }

        /// <inheritdoc />
        public abstract bool IsPrimary { get; /*protected set;*/ }

        /// <inheritdoc />
        public abstract IBlockIdentifier BlockIdentifier { get; }
    }
}
