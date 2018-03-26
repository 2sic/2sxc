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
        internal Folder Folder(string subFolder, bool autoCreate) 
            => AppContext.Folder(GeneratePath(subFolder), autoCreate);

        public string Root => GeneratePath("");


        protected abstract string GeneratePath(string subFolder);
        //{
        //    // Enable portal browsing if requested
        //    if (_usePortalRoot)
        //        return (subFolder ?? "").Replace("//", "/");
        //    var path = Configuration.ItemFolderMask
        //        .Replace("[AdamRoot]", AppContext.Path)
        //        .Replace("[Guid22]", Mapper.GuidCompress(_entityGuid))
        //        .Replace("[FieldName]", _fieldName)
        //        .Replace("[SubFolder]", subFolder) // often blank, so it will just be removed
        //        .Replace("//", "/"); // sometimes has duplicate slashes if subfolder blank but sub-sub is given
        //    return path;
        //}



        internal Folder Folder() => _folder ?? (_folder = Folder("", true));
        private Folder _folder;
        
    }
}