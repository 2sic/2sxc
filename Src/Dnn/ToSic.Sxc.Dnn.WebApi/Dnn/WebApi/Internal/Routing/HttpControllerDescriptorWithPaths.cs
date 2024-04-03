using System.Web.Http.Controllers;

namespace ToSic.Sxc.Dnn.WebApi.Internal;

internal class HttpControllerDescriptorWithPaths(HttpControllerDescriptor descriptor, string folder, string fullPath)
{
    public HttpControllerDescriptor Descriptor { get; } = descriptor;

    /// <summary>
    /// The folder in which the controller is located.
    /// For controllers inside AppCode, the folder is the AppCode folder.
    /// </summary>
    public string Folder { get; } = folder;

    /// <summary>
    /// The exact path to the controller file.
    /// Note that I'm not sure what it's actually used for.
    /// </summary>
    public string FullPath { get; } = fullPath;

}