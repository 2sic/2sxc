using ToSic.Lib.Services;

namespace ToSic.Sxc.Adam
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class AdamStorage: ServiceBase
    {
        protected AdamStorage(string logName) : base(logName)
        {
        }

        /// <summary>
        /// Root of this container
        /// </summary>
        public string Root => GeneratePath("");


        /// <summary>
        /// Figure out the path to a subfolder within this container
        /// </summary>
        /// <param name="subFolder"></param>
        /// <returns></returns>
        protected abstract string GeneratePath(string subFolder);

        
    }
}