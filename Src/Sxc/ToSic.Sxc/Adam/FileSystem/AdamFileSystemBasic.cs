using System;
using System.IO;
using ToSic.Eav;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// Basic implementation of the ADAM file system.
    /// This is string-based, not with environment IDs.
    /// It's primarily meant for standalone implementations or as a template for other integrations. 
    /// </summary>
    public partial class AdamFileSystemBasic: AdamFileSystemBase<string, string>, IAdamFileSystem<string, string>
    {
        #region Constructor / DI / Init

        public AdamFileSystemBasic(IAdamPaths adamPaths) : base(LogNames.Basic)
        {
            _adamPaths = adamPaths;
        }
        private readonly IAdamPaths _adamPaths;

        public IAdamFileSystem<string, string> Init(AdamManager<string, string> adamManager, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            AdamManager = adamManager;
            _adamPaths.Init(adamManager, Log);
            return this;
        }

        protected AdamManager<string, string> AdamManager;

        #endregion


        public int MaxUploadKb() => MaxUploadKbDefault;



        #region Helpers

        private string EnsurePhysicalPath(string path)
        {
            path = path.Backslash();
            return path.StartsWith("adam", StringComparison.CurrentCultureIgnoreCase)
                ? _adamPaths.PhysicalPath(path)
                : path;
        }


        /// <summary>
        /// When uploading a new file, we must verify that the name isn't used. 
        /// If it is used, walk through numbers to make a new name which isn't used. 
        /// </summary>
        /// <param name="parentFolder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string FindUniqueFileName(IFolder parentFolder, string fileName)
        {
            var callLog = Log.Fn<string>($"..., {fileName}");

            var name = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);
            for (var i = 1; i < MaxSameFileRetries && File.Exists(_adamPaths.PhysicalPath(Path.Combine(parentFolder.Path, Path.GetFileName(fileName)))); i++)
                fileName = $"{name}-{i}{ext}";
            
            return callLog.Return(fileName, fileName);
        }


        
        #endregion
    }
}
