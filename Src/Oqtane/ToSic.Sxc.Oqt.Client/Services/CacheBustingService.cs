using System;
using System.Linq;

namespace ToSic.Sxc.Oqt.Client.Services
{
    public class CacheBustingService
    {
        public static int Get(Guid renderId)
        {
            var bytes = renderId.ToByteArray();
            var sum = bytes.Aggregate(0, (current, b) => current + b);
            // Ensure the result is within the 7-digit range
            var sevenDigitInt = 1000000 + (sum % 9000000);
            return sevenDigitInt;
        }

        public string CacheBusting(string url, Guid renderId)
        {
            if (string.IsNullOrEmpty(url)) return "";

            if (!url.Contains('#')) return UrlWithCacheBusting(url, renderId);

            return UrlWithCacheBusting(url + "!", renderId);
        }

        private string UrlWithCacheBusting(string url, Guid renderId) => $"{url}#{Get(renderId)}";
    }
}
