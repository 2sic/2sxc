using System;
using ToSic.Eav.Apps.Assets;
using ToSic.SexyContent;
using ToSic.SexyContent.Adam;

namespace ToSic.Sxc.Adam
{
    public class File : Eav.Apps.Assets.File, IAsset, IFile, AdamFile
    {
        private AdamAppContext AppContext { get; }

        public File(AdamAppContext appContext)
        {
            AppContext = appContext;
        }

        /// <inheritdoc />
        public DynamicEntity Metadata => Adam.Metadata.GetFirstOrFake(AppContext, Id, false);

        /// <inheritdoc />
        public bool HasMetadata => Adam.Metadata.GetFirstMetadata(AppContext.App, Id, false) != null;

        /// <inheritdoc />
        public  string Url => AppContext.Tenant.ContentPath + Folder + FullName;

         /// <inheritdoc />
       public string Type => Classification.TypeName(Extension);

         /// <inheritdoc />
       public string Name { get; internal set; }


        #region Obsolete properties, included to ensure old stuff still works because of refactoring in 2sxc 9.20
        public string FileName => FullName;

        public DateTime CreatedOnDate => Created;

        public int FileId => Id;
        #endregion
    }
}