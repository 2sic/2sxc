using System;
using System.IO;
using ToSic.Eav.Run;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Hybrid
{
    public partial class RazorComponent<TModel>
    {
        public string CreateInstancePath
        {
            get => _createInstancePath ?? Path;
            set => _createInstancePath = value;
        }
        private string _createInstancePath;

        /// <summary>
        /// Creates instances of the shared pages with the given relative path
        /// </summary>
        /// <returns></returns>
        public dynamic CreateInstance(string virtualPath,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string name = null,
            string relativePath = null,
            bool throwOnError = true)
        {
            var directory = System.IO.Path.GetDirectoryName(Path);
            if (directory == null) throw new Exception("Current directory seems to be null");
            var path = System.IO.Path.Combine(directory, virtualPath);
            VerifyFileExists(path);
            //return "all is ok with " + virtualPath + "(" + path + ")";
            
            return path.EndsWith(CodeCompiler.CsFileExtension)
                ? _DynCodeRoot.CreateInstance(path, dontRelyOnParameterOrder, name, null, throwOnError)
                : throw new NotImplementedException(); 
            // CreateInstanceCshtml(path);
        }

        protected dynamic CreateInstanceCshtml(string path)
        {
            
            throw new NotImplementedException();
            //var webPage = this. CreateInstanceFromVirtualPath(path);
            //webPage.ConfigurePage(this);
            //return webPage;
        }

        protected void VerifyFileExists(string path)
        {
            var pathFinder = GetService<IServerPaths>();
            var finalPath = pathFinder.FullAppPath(path);
            if (!File.Exists(finalPath))
                throw new FileNotFoundException("The shared file does not exist.", path);
        }
    }
}
