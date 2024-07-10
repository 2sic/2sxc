using System;
using System.Collections.Generic;

namespace ToSic.Sxc.Oqt.Client.Services
{
    public class CacheBustingService
    {
        private readonly Dictionary<int, int> _pageNumbers = [];
        private readonly Random _random = new();

        /// <summary>
        /// Get a unique nocache number for a given page number.
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns>int</returns>
        public int Get(int pageId)
        {
            // Check if a unique number has already been generated for the given page number.
            if (_pageNumbers.TryGetValue(pageId, out var uniqueRandomNumberPerPage)) 
                return uniqueRandomNumberPerPage;

            // Generate a unique nocache number that isn't already a value in the dictionary.
            int uniqueNumber;
            do
            {
                uniqueNumber = _random.Next(1000000, 9999999);
            } while (_pageNumbers.ContainsValue(uniqueNumber));

            // store unique number to the dictionary.
            _pageNumbers[pageId] = uniqueNumber;

            return _pageNumbers[pageId];
        }

        public string CacheBusting(string url, int pageId)
        {
            // add ?nocache=uniqueNumber to the url if url do not contain ?
            if (string.IsNullOrEmpty(url)) return "";

            if (!url.Contains('#')) return UrlWithCacheBusting(url, pageId);

            // split url by # and add nocache number to the first part of the url
            var urlParts = url.Split("#");
            return $"{UrlWithCacheBusting(urlParts[0], pageId)}#{urlParts[1]}";
        }

        private string UrlWithCacheBusting(string url, int pageId) 
            => url.Contains('?') ? $"{url}&nocache={Get(pageId)}" : $"{url}?nocache={Get(pageId)}";
    }
}
