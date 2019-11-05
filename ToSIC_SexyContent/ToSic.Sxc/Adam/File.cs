using System;
using ToSic.Eav.Apps.Assets;
using ToSic.SexyContent.Adam;
using ToSic.Sxc.Interfaces;

namespace ToSic.Sxc.Adam
{
    public class File : Eav.Apps.Assets.File, AdamFile, IAdamFile
    {
        private AdamAppContext AppContext { get; }

        public File(AdamAppContext appContext)
        {
            AppContext = appContext;
        }

        /// <inheritdoc cref="IDynamicEntity" />
        public dynamic Metadata => Adam.Metadata.GetFirstOrFake(AppContext, Id, false) as dynamic;

        public bool HasMetadata => Adam.Metadata.GetFirstMetadata(AppContext.AppRuntime, Id, false) != null;

        public string Url => AppContext.Tenant.ContentPath + Folder + FullName;

       public string Type => Classification.TypeName(Extension);



        #region Obsolete properties, included to ensure old stuff still works because of refactoring in 2sxc 9.20
        public string FileName => FullName;

        public DateTime CreatedOnDate => Created;

        public int FileId => Id;
        #endregion
    }
}