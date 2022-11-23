using ToSic.Eav.Context;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.WebApi.Save
{
    /// <summary>
    /// All save helpers usually need the sxc-instance and the log
    /// </summary>
    public abstract class SaveHelperBase<T>: HasLog where T: SaveHelperBase<T>
    {
        internal IContextOfApp Context { get; private set; }

        protected SaveHelperBase(string logName)  : base( logName ) { }

        public T Init(IContextOfApp context, ILog parentLog)
        {
            (Log as Log)?.LinkTo(parentLog);
            Context = context;
            return this as T;
        }
    }
}
