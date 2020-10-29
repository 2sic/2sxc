using System;
using System.Collections.Generic;
using System.Text;

namespace ToSic.Sxc.Oqt.Server.Adam
{
    internal static class PathFixer
    {
        public static string Backslash(this string original)
            => original.Replace("/", "\\").Replace("\\\\", "\\");

        public static string Forwardslash(this string original)
            => original.Replace("\\", "/").Replace("//", "/").Replace("//", "/");

    }
}
