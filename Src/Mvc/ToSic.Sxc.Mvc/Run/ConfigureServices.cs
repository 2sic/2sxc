using ToSic.Eav.Plumbing.Booting;

namespace ToSic.Sxc.Mvc.Run
{
    public class ConfigureServices: IConfigurationLoader
    {
        /// <summary>
        /// This is a special callback which was needed by DNN to auto-load certain features.
        /// MVC probably won't need this, as we can do our startup in a more controlled way. 
        /// </summary>
        public void Configure()
        {
            /* ignore */
        }
    }
}
