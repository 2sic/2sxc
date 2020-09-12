namespace ToSic.Sxc.Adam
{
    public abstract class AdamOfBase<TFolderId, TFileId>: ContainerBase
    {
        protected AdamOfBase(AdamAppContext<TFolderId, TFileId> appContext)
        {
            AppContext = appContext;
        }
        public readonly AdamAppContext<TFolderId, TFileId> AppContext;


        /// <summary>
        /// Get the folder specified in App.Settings (BasePath) combined with the module's ID
        /// </summary>
        /// <remarks>
        /// Will create the folder if it does not exist
        /// </remarks>
        internal Folder<TFolderId, TFileId> Folder(string subFolder, bool autoCreate)
            => AppContext.Folder(GeneratePath(subFolder), autoCreate);


        /// <summary>
        /// Get a (root) folder object for this container
        /// </summary>
        /// <returns></returns>
        internal Folder<TFolderId, TFileId> Folder() => _folder ?? (_folder = Folder("", true));
        private Folder<TFolderId, TFileId> _folder;

    }
}
