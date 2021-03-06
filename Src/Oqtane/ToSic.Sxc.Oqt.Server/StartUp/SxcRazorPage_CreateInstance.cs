﻿using System;

namespace ToSic.Sxc.Oqt.Server.RazorPages
{
    public partial class SxcRazorPage<TModel>
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
            throw new NotImplementedException();
            //var path = NormalizePath(virtualPath);
            //VerifyFileExists(path);
            //return path.EndsWith(CodeCompiler.CsFileExtension)
            //    ? DynCode.CreateInstance(path, dontRelyOnParameterOrder, name, null, throwOnError)
            //    : CreateInstanceCshtml(path);
        }

        protected dynamic CreateInstanceCshtml(string path)
        {
            throw new NotImplementedException();
            //var webPage = (RazorComponentBase)CreateInstanceFromVirtualPath(path);
            //webPage.ConfigurePage(this);
            //return webPage;
        }

        protected static void VerifyFileExists(string path)
        {
            throw new NotImplementedException();
            //if (!File.Exists(HostingEnvironment.MapPath(path)))
            //    throw new FileNotFoundException("The shared file does not exist.", path);
        }
    }
}
