using ToSic.Eav.Internal.Environment;
using ToSic.Lib.DI;
#if NETFRAMEWORK
using System.Web.Hosting;
#else
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
#endif


namespace ToSic.Sxc.Integration.Paths;

/// <summary>
/// In the default implementation, all things have the same root so content-path and app-path
/// are calculated the same way.
/// </summary>
internal class ServerPaths: ServerPathsBase
{
#if !NETFRAMEWORK

    // NOTE: The .net standard is probably never used
    // As Oqtane has it's own ServerPaths
    // So we should probably drop this soon
    // ReSharper disable once ConvertToPrimaryConstructor
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
    // ReSharper disable once ConvertToPrimaryConstructor
    public ServerPaths(LazySvc<IValueConverter> valueConverterLazy)
        {
            _valueConverterLazy = valueConverterLazy;
        }
        private readonly LazySvc<IValueConverter> _valueConverterLazy;

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