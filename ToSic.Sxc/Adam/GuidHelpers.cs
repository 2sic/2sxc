using System;

namespace ToSic.SexyContent.Adam
{
    internal static class GuidHelpers
    {
        internal static string Compress22(Guid newGuid)
        {
            var modifiedBase64 = Convert.ToBase64String(newGuid.ToByteArray())
                .Replace('+', '-').Replace('/', '_')    // avoid invalid URL characters
                .Substring(0, 22);                      // truncate trailing "==" characters
            return modifiedBase64;
        }

        internal static Guid Uncompress22(string shortGuid)
        {
            var base64 = shortGuid.Replace('-', '+').Replace('_', '/') + "==";
            var bytes = Convert.FromBase64String(base64);
            return new Guid(bytes);
        }

    }
}