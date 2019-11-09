using System;
using ToSic.SexyContent.Adam;

namespace ToSic.Sxc.Adam
{
    public class File : Eav.Apps.Assets.File, 
#pragma warning disable 618
        AdamFile, 
#pragma warning restore 618
        IFile
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
        [Obsolete("use FullName instead")]
        public string FileName => FullName;

        [Obsolete("use Created instead")]
        public DateTime CreatedOnDate => Created;

        [Obsolete("use Id instead")]
        public int FileId => Id;
        #endregion
    }
}