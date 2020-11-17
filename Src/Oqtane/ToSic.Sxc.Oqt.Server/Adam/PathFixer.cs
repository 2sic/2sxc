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
            if (original.StartsWith('\\')) original = original.TrimStart('\\');
            return "/" + original;
        }
    }
}
