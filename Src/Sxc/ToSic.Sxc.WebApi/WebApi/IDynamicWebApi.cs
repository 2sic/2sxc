using System.IO;
using ToSic.Lib.Coding;
using ToSic.Sxc.Adam;

// ReSharper disable UnusedMember.Global

namespace ToSic.Sxc.WebApi;

/// <summary>
/// This interface extends WebAPIs with File-Save helpers.
/// It's important, because if 2sxc also runs on other CMS platforms, then the Dnn Context won't be available, so it's in a separate interface.
/// </summary>
[PrivateApi("Was public till v17, but now the docs are all objects directly")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IDynamicWebApi
{
    /// <summary>
    /// Save a file from a stream (usually an upload from the browser) into an adam-field of an item.
    /// Read more about this in the [WebAPI docs for SaveInAdam](xref:NetCode.WebApi.DotNet.SaveInAdam)
    /// </summary>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="stream">the stream</param>
    /// <param name="fileName">file name to save to</param>
    /// <param name="contentType">content-type of the target item (important for security checks)</param>
    /// <param name="guid"></param>
    /// <param name="field"></param>
    /// <param name="subFolder"></param>
    /// <returns></returns>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    IFile SaveInAdam(NoParamOrder noParamOrder = default,
        Stream stream = null,
        string fileName = null,
        string contentType = null,
        Guid? guid = null,
        string field = null,
        string subFolder = "");


    /// <summary>
    /// Create a File-result to stream to the client
    ///
    ///
    /// Typical use: `return File(download: true, contentType: "text/xml", contents: ...);`
    /// </summary>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="download">If a download should be enforced (otherwise the file may just be displayed - like an image)</param>
    /// <param name="virtualPath">Path in the website to get the file from. Provide _either_ virtualPath or contents</param>
    /// <param name="contentType">Mime Content-type. Will try to auto-detect from virtualPath or fileDownloadName if not provided.</param>
    /// <param name="fileDownloadName">Download name. If provided, it will try to force download/save on the browser. </param>
    /// <param name="contents">Content of the result - a string, byte[] or stream to include.</param>
    /// <returns></returns>
    /// <remarks>
    /// Added in 2sxc 12.05
    /// </remarks>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    dynamic File(NoParamOrder noParamOrder = default,
        // Important: the second parameter should _not_ be a string, otherwise the signature looks the same as the built-in File(...) method
        bool? download = null,
        // important: this is the virtualPath, but it should not have the same name, to not confuse the compiler with same sounding param names
        string virtualPath = null,
        string contentType = null,
        string fileDownloadName = null,
        object contents = null
    );

}