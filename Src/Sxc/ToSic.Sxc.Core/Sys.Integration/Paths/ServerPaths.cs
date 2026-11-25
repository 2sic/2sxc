using System.Diagnostics.CodeAnalysis;
using ToSic.Eav.Data.Sys.ValueConverter;
using ToSic.Eav.Environment.Sys.ServerPaths;
#if NETFRAMEWORK
using System.Web.Hosting;
#else
using Microsoft.AspNetCore.Hosting;
#endif


namespace ToSic.Sxc.Sys.Integration.Paths;

/// <summary>
/// In the default implementation, all things have the same root so content-path and app-path
/// are calculated the same way.
/// </summary>
internal class ServerPaths: IServerPaths
{
#if !NETFRAMEWORK

    // NOTE: The .net standard is probably never used
    // As Oqtane has it's own ServerPaths
    // So we should probably drop this soon
    // ReSharper disable once ConvertToPrimaryConstructor
    public ServerPaths(IWebHostEnvironment hostingEnvironment)
    {
        _hostingEnvironment = hostingEnvironment;
    }

    private readonly IWebHostEnvironment _hostingEnvironment;

    protected string MapContentPath(string virtualPath)
    {
        return Path.Combine(_hostingEnvironment.ContentRootPath, virtualPath);
    }

    protected string? FullPathOfReference(int id)
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

    private string MapContentPath(string virtualPath)
        => HostingEnvironment.MapPath(virtualPath) ?? "error mapping path.";

    private string? FullPathOfReference(int id)
    {
        var fileRef = "file:" + id;
        var resolved = _valueConverterLazy.Value.ToValue(fileRef);
        return string.IsNullOrWhiteSpace(resolved)
            ? null
            : FullContentPath(resolved);
    }
#endif


    /// <inheritdoc />
    public string FullAppPath(string virtualPath) => MapContentPath(virtualPath);

    /// <inheritdoc />
    public string FullContentPath(string virtualPath) => MapContentPath(virtualPath);

    [return: NotNullIfNotNull("fileReference")]
    public string? FullPathOfReference(string fileReference) => ServerPathsShared.FullPathOfReference(fileReference, FullPathOfReference);
}