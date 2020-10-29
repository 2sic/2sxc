using ToSic.Eav.Plumbing.Booting;

namespace ToSic.Sxc.Oqt.Server.Run
{
    /// <summary>
    /// This is just an empty class that doesn't do anything
    /// Would initialize some stuff if necessary. 
    /// </summary>
    public class OqtConfigurationLoader: IConfigurationLoader
    {
        /// <summary>
        /// This is a special callback which was needed by DNN to auto-load certain features.
        /// Oqtane probably won't need this, as we can do our startup in a more controlled way. 
        /// </summary>
        public void Configure()
        {
            /* ignore */
        }
    }
}
