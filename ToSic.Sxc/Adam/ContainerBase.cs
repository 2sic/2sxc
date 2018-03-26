using ToSic.Eav.Apps.Assets;

namespace ToSic.Sxc.Adam
{
    public abstract class ContainerBase
    {
        public readonly AdamAppContext AppContext;

        protected ContainerBase(AdamAppContext appContext)
        {
            AppContext = appContext;
        }


        /// <summary>
        /// Get the folder specified in App.Settings (BasePath) combined with the module's ID
        /// </summary>
        /// <remarks>
        /// Will create the folder if it does not exist
        /// </remarks>
        internal Eav.Apps.Assets.Folder Folder(string subFolder, bool autoCreate) 
            => AppContext.Folder(GeneratePath(subFolder), autoCreate);

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

        /// <summary>
        /// Get a (root) folder object for this container
        /// </summary>
        /// <returns></returns>
        internal Eav.Apps.Assets.Folder Folder() => _folder ?? (_folder = Folder("", true));
        private Eav.Apps.Assets.Folder _folder;
        
    }
}