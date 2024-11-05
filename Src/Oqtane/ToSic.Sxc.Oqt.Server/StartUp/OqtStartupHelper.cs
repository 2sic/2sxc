using System;
using static ToSic.Sxc.Oqt.Server.WebApi.OqtWebApiConstants;

namespace ToSic.Sxc.Oqt.Server.StartUp;

internal static class OqtStartupHelper
{

    public static bool IsSxcEndpoint(string path) => SxcEndpointPathRegex.IsMatch(path);

    public static bool IsSxcDialog(string path) => SxcDialogs.Any(p => path.Contains(p.url, StringComparison.OrdinalIgnoreCase));
}