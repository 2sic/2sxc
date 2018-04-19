using System.Configuration;
using System.IO;
using System.Web.Configuration;
using DotNetNuke.Services.FileSystem;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Adam.WebApi
{
    public partial class AdamController
    {
        /// <summary>
        /// The configuration max-file-upload size
        /// </summary>
        /// <remarks>
        /// if not specified, go to very low value, but not 0, as that could be infinite...
        /// </remarks>
        public int MaxFileSizeKb
            => (ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection)
               ?.MaxRequestLength ?? 1;


        //internal ContainerBase ContainerContext;
        //internal AdamAppContext AdamAppContext;

        //private void PrepCore(App app, Guid entityGuid, string fieldName, bool usePortalRoot)
        //{
        //    var dnn = new DnnHelper(SxcInstance?.EnvInstance);
        //    var tenant = new DnnTenant(dnn.Portal);
        //    AdamAppContext = new AdamAppContext(tenant, app, SxcInstance);
        //    ContainerContext = usePortalRoot
        //        ? new ContainerOfTenant(AdamAppContext) as ContainerBase
        //        : new ContainerOfField(AdamAppContext, entityGuid, fieldName);
        //}

        //private IAttributeDefinition Definition(int appId, string contentType, string fieldName)
        //{
        //    // try to find attribute definition - for later extra security checks
        //    var appRead = new AppRuntime(appId, Log);
        //    var type = appRead.ContentTypes.Get(contentType);
        //    return type[fieldName];
        //}


        private static string RenameFileToNotOverwriteExisting(string fileName, IFolderInfo dnnFolder)
        {
            var numberedFile = fileName;

            bool FileExists(string fileToCheck) => FileManager.Instance
                .FileExists(dnnFolder, Path.GetFileName(fileToCheck));

            for (var i = 1; i < 1000 && FileExists(numberedFile); i++)
                numberedFile = Path.GetFileNameWithoutExtension(fileName)
                               + "-" + i + Path.GetExtension(fileName);
            fileName = numberedFile;
            return fileName;
        }
    }
}
