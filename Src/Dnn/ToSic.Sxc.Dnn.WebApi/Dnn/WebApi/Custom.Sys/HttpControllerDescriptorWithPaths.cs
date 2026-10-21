using System.Web.Http.Controllers;

namespace ToSic.Sxc.Dnn.WebApi.Custom.Sys;

/// <summary>
/// Temp object to pass around the controller descriptor and the paths to the controller file.
/// </summary>
/// <param name="Descriptor">
/// The .net controller descriptor, which contains the controller type and other information.
/// </param>
/// <param name="Folder">
/// The folder in which the controller is located.
/// For controllers inside AppCode, the folder is the AppCode folder.
/// </param>
/// <param name="FullPath">
/// The exact path to the controller file.
/// Note that I'm not sure what it's actually used for.
/// </param>
internal record HttpControllerDescriptorWithPaths(HttpControllerDescriptor Descriptor, string Folder, string FullPath);