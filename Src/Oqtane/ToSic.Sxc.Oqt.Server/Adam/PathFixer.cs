using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace ToSic.Sxc.Oqt.Server.Adam
{
    internal static class PathFixer
    {
        public static string Backslash(this string original)
            => original.Replace("/", "\\").Replace("\\\\", "\\");

        public static string Forwardslash(this string original)
            => original.Replace("\\", "/").Replace("//", "/").Replace("//", "/");

        public static string PrefixSlash(this string original)
        {
            if (original == null) return "/";
            if (original.StartsWith('/')) return original;
            return "/" + original;
        }
    }
}
