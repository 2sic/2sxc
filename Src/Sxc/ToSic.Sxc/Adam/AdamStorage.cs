using ToSic.Lib.Logging;

namespace ToSic.Sxc.Adam
{
    public abstract class AdamStorage: HasLog<AdamStorage>
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