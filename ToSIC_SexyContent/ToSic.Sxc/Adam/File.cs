using System;
using ToSic.Eav.Apps.Assets;
using ToSic.SexyContent.Adam;
using ToSic.Sxc.Interfaces;

namespace ToSic.Sxc.Adam
{
    public class File : Eav.Apps.Assets.File, IAsset, AdamFile, IFile
    {
        private AdamAppContext AppContext { get; }

        public File(AdamAppContext appContext)
        {
            AppContext = appContext;
        }

        /// <inheritdoc cref="IDynamicEntity" />
        public IDynamicEntity Metadata => Adam.Metadata.GetFirstOrFake(AppContext, Id, false);

        /// <inheritdoc cref="IDynamicEntity" />
        public bool HasMetadata => Adam.Metadata.GetFirstMetadata(AppContext.AppRuntime, Id, false) != null;

        /// <inheritdoc cref="IDynamicEntity" />
        public string Url => AppContext.Tenant.ContentPath + Folder + FullName;

        /// <inheritdoc cref="IDynamicEntity" />
       public string Type => Classification.TypeName(Extension);

        /// <inheritdoc cref="IDynamicEntity" />
       public string Name { get; internal set; }


        #region Obsolete properties, included to ensure old stuff still works because of refactoring in 2sxc 9.20
        public string FileName => FullName;

        public DateTime CreatedOnDate => Created;

        public int FileId => Id;
        #endregion
    }
}