#if NETSTANDARD
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
#else
using System.Web.Hosting;
using System;
using ToSic.Eav.Data;
#endif
using ToSic.Eav.Run;


namespace ToSic.Sxc.Run
{
    /// <summary>
    /// In the default implementation, all things have the same root so content-path and app-path
    /// are calculated the same way.
    /// </summary>
    public class ServerPaths: ServerPathsBase
    {
#if NETSTANDARD

        // NOTE: The .net standard is probably never used
        // As Oqtane has it's own ServerPaths
        // So we should probably drop this soon
        public ServerPaths(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        private readonly IHostingEnvironment _hostingEnvironment;

        protected string MapContentPath(string virtualPath)
        {
            return Path.Combine(_hostingEnvironment.ContentRootPath, virtualPath);
        }

        protected override string FullPathOfReference(int id)
        {
            throw new NotImplementedException("leave as not implemented, as we assume this code is never ever used");
        }

#else
        public ServerPaths(Lazy<IValueConverter> valueConverterLazy)
        {
            _valueConverterLazy = valueConverterLazy;
        }
        private readonly Lazy<IValueConverter> _valueConverterLazy;

        protected string MapContentPath(string virtualPath) => HostingEnvironment.MapPath(virtualPath);

        protected override string FullPathOfReference(int id)
        {
            var fileRef = "file:" + id;
            var resolved = _valueConverterLazy.Value.ToValue(fileRef);
            if (string.IsNullOrWhiteSpace(resolved)) return null;
            return FullContentPath(resolved);
        }
#endif


        /// <inheritdoc />
        public override string FullAppPath(string virtualPath) => MapContentPath(virtualPath);

        /// <inheritdoc />
        public override string FullContentPath(string virtualPath) => MapContentPath(virtualPath);

    }
}
