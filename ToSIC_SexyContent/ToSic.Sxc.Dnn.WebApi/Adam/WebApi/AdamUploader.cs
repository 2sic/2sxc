using DotNetNuke.Services.FileSystem;
using System;
using System.Configuration;
using System.IO;
using System.Web.Configuration;
using System.Web.Http;
using ToSic.Eav.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.WebApi;


namespace ToSic.Sxc.Adam.WebApi
{
    internal class AdamUploader: HasLog
    {
        protected readonly IBlockBuilder BlockBuilder;
        private readonly int _appId;

        public AdamUploader(IBlockBuilder blockBuilder, int appId, ILog parentLog) : base("Api.AdmUpl", parentLog)
        {
            BlockBuilder = blockBuilder;
            _appId = appId;
        }

        public IFile UploadOne(Stream stream, string originalFileName, string contentType, Guid guid, string field, string subFolder, bool usePortalRoot, bool skipFieldAndContentTypePermissionCheck)
        {
            Log.Add($"upload one a:{_appId}, i:{guid}, field:{field}, subfold:{subFolder}, useRoot:{usePortalRoot}");
            
            var state = new AdamSecureState(BlockBuilder, _appId, contentType, field, guid, usePortalRoot, Log);
            HttpResponseException exp;
            if (!skipFieldAndContentTypePermissionCheck)
            {
                if(!state.UserIsPermittedOnField(GrantSets.WriteSomething, out exp))
                    throw exp;

                // check that if the user should only see drafts, he doesn't see items of published data
                if (!state.UserIsNotRestrictedOrItemIsDraft(guid, out var permissionException))
                    throw permissionException;
            }

            var folder = state.ContainerContext.Folder();

            if (!string.IsNullOrEmpty(subFolder))
                folder = state.ContainerContext.Folder(subFolder, false);
            
            // start with a security check...
            var dnnFolder = FolderManager.Instance.GetFolder(folder.Id);

            // validate that dnn user have write permissions for folder in case dnn file system is used (usePortalRoot)
            if (usePortalRoot && !SecurityChecks.CanEdit(dnnFolder))
                throw Http.PermissionDenied("can't upload - permission denied");

            // we only upload into valid adam if that's the scenario
            if (!state.SuperUserOrAccessingItemFolder(dnnFolder.PhysicalPath, out exp))
                throw exp;

            #region check content-type extensions...

            // Check file size and extension
            var fileName = String.Copy(originalFileName);
            if(!state.ExtensionIsOk(fileName, out exp))
                throw exp;

            // check metadata of the FieldDef to see if still allowed extension
            var additionalFilter = state.Attribute.Metadata.GetBestValue<string>("FileFilter");
            if (!string.IsNullOrWhiteSpace(additionalFilter)
                && !state.CustomFileFilterOk(additionalFilter, originalFileName))
                throw Http.NotAllowedFileType(fileName, "field has custom file-filter, which doesn't match");

            // note 2018-04-20 2dm: can't do this for wysiwyg, as it doesn't have a setting for allowed file-uploads

            #endregion

            if (stream.Length > 1024 * MaxFileSizeKb)
                throw new Exception($"file too large - more than {MaxFileSizeKb}Kb");

            // remove forbidden / troubling file name characters
            fileName = fileName
                .Replace("+", "plus")
                .Replace("%", "per")
                .Replace("#", "hash");

            if (fileName != originalFileName)
                Log.Add($"cleaned file name from'{originalFileName}' to '{fileName}'");

            // Make sure the image does not exist yet, cycle through numbers (change file name)
            fileName = RenameFileToNotOverwriteExisting(fileName, dnnFolder);

            // Everything is ok, add file to DNN
            var dnnFile = FileManager.Instance.AddFile(dnnFolder, Path.GetFileName(fileName),
                stream);

            var adamcontext = new AdamAppContext(BlockBuilder.Block.Context.Tenant, state.App, BlockBuilder, 10, Log);
            var eavFile = new File(adamcontext)
            {
                Created = dnnFile.CreatedOnDate,
                Extension = dnnFile.Extension,
                FullName = dnnFile.FileName,
                Folder = dnnFile.Folder,
                FolderId = dnnFile.FolderId,
                Id = dnnFile.FileId,
                Modified = dnnFile.LastModificationTime
            };

            return eavFile;
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



        internal static string RenameFileToNotOverwriteExisting(string fileName, IFolderInfo dnnFolder)
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

