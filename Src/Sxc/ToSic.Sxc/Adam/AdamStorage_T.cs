namespace ToSic.Sxc.Adam
{
    public abstract class AdamStorage<TFolderId, TFileId>: AdamStorage
    {
        protected AdamStorage(AdamManager<TFolderId, TFileId> manager): base("Adm.Base")
        {
            Manager = manager;
        }
        public readonly AdamManager<TFolderId, TFileId> Manager;


        /// <summary>
        /// Get the folder specified in App.Settings (BasePath) combined with the module's ID
        /// </summary>
        /// <remarks>
        /// Will create the folder if it does not exist
        /// </remarks>
        internal Folder<TFolderId, TFileId> Folder(string subFolder, bool autoCreate)
            => Log.Intercept($"{nameof(Folder)}({subFolder}, {autoCreate})", () => Manager.Folder(GeneratePath(subFolder), autoCreate));


        /// <summary>
        /// Get a (root) folder object for this container
        /// </summary>
        /// <returns></returns>
        internal Folder<TFolderId, TFileId> Folder() => _folder ?? (_folder = Folder("", true));
        private Folder<TFolderId, TFileId> _folder;

    }
}
