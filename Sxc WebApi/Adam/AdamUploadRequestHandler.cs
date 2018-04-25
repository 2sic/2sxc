using DotNetNuke.Services.FileSystem;
using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.WebApi.Errors;
using ToSic.Sxc.Adam.WebApi;


namespace ToSic.SexyContent.WebApi.Adam
{
    public class AdamUploadRequestHandler
    {
        public IFileInfo UploadOne(Stream stream, string originalFileName, SxcInstance sxcInstance, Eav.Logging.Simple.Log log, int appId, string contentType, Guid guid, string field, string subFolder, bool usePortalRoot)
        {
            log.Add($"upload one a:{appId}, i:{guid}, field:{field}, subfold:{subFolder}, useRoot:{usePortalRoot}");
            
            var state = new AdamSecureState(sxcInstance, appId, contentType, field, guid, usePortalRoot, log);
            state.ThrowIfRestrictedUserIsntPermittedOnField(GrantSets.WriteSomething);

            var folder = state.ContainerContext.Folder();

            if (!string.IsNullOrEmpty(subFolder))
                folder = state.ContainerContext.Folder(subFolder, false);
            
            // start with a security check - so we only upload into valid adam if that's the scenario
            var dnnFolder = FolderManager.Instance.GetFolder(folder.Id);
            state.ThrowIfRestrictedUserIsOutsidePermittedFolders(dnnFolder.PhysicalPath);

            #region check content-type extensions...

            // Check file size and extension
            var fileName = String.Copy(originalFileName);
            state.ThrowIfBadExtension(fileName);

            // todo: check metadata of the FieldDef to see if still allowed extension
            var additionalFilter = state.Attribute.Metadata.GetBestValue<string>("FileFilter");
            if (!string.IsNullOrWhiteSpace(additionalFilter)
                && !state.CustomFileFilterOk(additionalFilter, originalFileName))
                throw Http.NotAllowedFileType(fileName, "field has custom file-filter, which doesn't match");

            // note 2018-04-20 2dm: can't do this for wysiwyg, as it doesn't have a setting for allowed file-uploads

            #endregion

            if (stream.Length > 1024 * MaxFileSizeKb)
                throw new Exception($"file too large - more than {MaxFileSizeKb}Kb");

            // remove forbidden / troubling file name characters
            fileName = fileName.Replace("%", "per").Replace("#", "hash");

            if (fileName != originalFileName)
                log.Add($"cleaned file name from'{originalFileName}' to '{fileName}'");

            // Make sure the image does not exist yet, cycle through numbers (change file name)
            fileName = RenameFileToNotOverwriteExisting(fileName, dnnFolder);

            // Everything is ok, add file
            var dnnFile = FileManager.Instance.AddFile(dnnFolder, Path.GetFileName(fileName),
                stream);

            return dnnFile;
        }

        /// <summary>
        /// The configuration max-file-upload size
        /// </summary>
        /// <remarks>
        /// if not specified, go to very low value, but not 0, as that could be infinite...
        /// </remarks>
        public static int MaxFileSizeKb
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


        public static string RenameFileToNotOverwriteExisting(string fileName, IFolderInfo dnnFolder)
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
