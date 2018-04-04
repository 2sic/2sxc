using ToSic.Eav.Apps.Assets;
using ToSic.SexyContent;

namespace ToSic.Sxc.Adam
{
    public class File : Eav.Apps.Assets.File, IAsset
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
    }
}