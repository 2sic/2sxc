using ToSic.Eav.Context;
using ToSic.Lib.Services;

namespace ToSic.Sxc.WebApi.Save
{
    /// <summary>
    /// All save helpers usually need the sxc-instance and the log
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class SaveHelperBase: ServiceBase
    {
        internal IContextOfApp Context { get; private set; }

        protected SaveHelperBase(string logName)  : base( logName ) { }

        public void Init(IContextOfApp context)
        {
            Context = context;
        }
    }
}
