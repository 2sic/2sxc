using System.IO;
using DotNetNuke.Common;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Context;

namespace ToSic.Sxc.Dnn.Context;

internal static class DnnISiteExtensions
{
    internal static string SharedAppsRootRelative(this ISite site) => Path.Combine(Globals.HostPath, AppConstants.AppsRootFolder);
}