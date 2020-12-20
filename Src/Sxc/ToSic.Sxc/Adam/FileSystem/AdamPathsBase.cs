using System;
using System.IO;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Adam
{
    public abstract class AdamPathsBase: HasLog, IAdamPaths
    {
        #region DI Constructor & Init

        protected AdamPathsBase(IServerPaths serverPaths, string logPrefix) : base($"{logPrefix}.AdmPth")
        {
            _serverPaths = serverPaths;
        }
        private readonly IServerPaths _serverPaths;

        public IAdamPaths Init(AdamManager adamManager, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            AdamManager = adamManager;
            return this;
        }
        protected AdamManager AdamManager { get; private set; }

        #endregion

        // todo: continue here
        // test
        // continue with the other commands

        public string PhysicalPath(string path)
        {
            if (path.Contains("..")) throw new ArgumentException("path may not contain ..", nameof(path));
            // check if it already has the root path attached, otherwise add
            path = path.StartsWith(AdamManager.Site.ContentPath) ? path : Path.Combine(AdamManager.Site.ContentPath, path);
            return _serverPaths.FullContentPath(path.Backslash());
        }

        public string RelativeFromAdam(string path)
        {
            var pathForSearch = path.Forwardslash();
            var adamPosition = pathForSearch.IndexOf("adam/", StringComparison.InvariantCultureIgnoreCase);
            return adamPosition < 0 
                ? path 
                : path.Substring(adamPosition);
        }

        //public string PhysicalPath(IAsset asset)
        //{
        //    throw new NotImplementedException();
        //}

        public virtual string Url(string path) => Path.Combine(AdamManager.Site.ContentPath, path).Forwardslash();

        //public string Url(IAsset asset)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
