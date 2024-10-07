using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using ToSic.Eav.Caching;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Code.Internal.HotBuild
{
    public class SourceCodeHasher(LazySvc<IPlatformInfo> platform, MemoryCacheService memoryCacheService) : ServiceBase("Sxc.ScCdHsh", connect: [memoryCacheService])
    {
        private const string CsFiles = ".cs";
        private const bool UseSubfolders = true;
        private const int BufferSize = 16 * 1024; // 16KB buffer
        private const int CacheMinutes = 10;

        public string GetHashString(string folderPath)
        {
            var l = Log.Fn<string>($"{nameof(folderPath)}: '{folderPath}'", timer: true);

            // See if in memory cache
            var cacheKey = FolderHashCacheKey(folderPath);
            if (memoryCacheService.TryGet<string>(cacheKey, out var fromCache))
                return l.Return(fromCache, "from cache");

            var files = GetSourceFilesInFolder(folderPath);
            l.A($"{files.Length} files found");

            var computeHashStringForFiles = BitConverter.ToString(ComputeHashForFiles(files)).Replace("-", "").ToLower();
            l.A("hash string computed");

            memoryCacheService.SetNew(cacheKey, files, p => p
                .SetSlidingExpiration(new(0, CacheMinutes, 0))
                .WatchCacheKeys(new[] { SourceFilesInFolderCacheKey(folderPath) }));

            return l.ReturnAsOk(computeHashStringForFiles);
        }

        internal string[] GetSourceFilesInFolder(string fullPath)
        {
            var l = Log.Fn<string[]>($"{nameof(fullPath)}: '{fullPath}'", timer: true);

            // See if in memory cache
            var cacheKey = SourceFilesInFolderCacheKey(fullPath);
            if (memoryCacheService.TryGet<string[]>(cacheKey, out var fromCache))
                return l.Return(fromCache, "from cache");

            var files = Directory.GetFiles(fullPath, $"*{CsFiles}", UseSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            l.A($"{files.Length} files found with {nameof(UseSubfolders)}: {UseSubfolders}");
            Array.Sort(files, StringComparer.Ordinal); // sort the array of file paths before processing to ensure hashing deterministic behavior.
            l.A("sorted files");

            memoryCacheService.SetNew(cacheKey, files, p => p
                .SetSlidingExpiration(new(0, CacheMinutes, 0))
                .WatchFolders(new Dictionary<string, bool> { { fullPath, UseSubfolders } }));

            return l.ReturnAsOk(files);
        }

        private string FolderHashCacheKey(string fullPath) => $"Sxc-FolderHash-{fullPath}";

        private string SourceFilesInFolderCacheKey(string fullPath) => $"Sxc-SourceFilesInFolder-{fullPath}";

        /// <summary>
        /// Create a hash of all files which are relevant for the DLL.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private byte[] ComputeHashForFiles(string[] files)
        {
            var l = Log.Fn<byte[]>($"{nameof(files)}: {files.Count()}", timer: true);

            using var hashAlgorithm = SHA256.Create(); // fips compatibility

            HashRelevantKeys(hashAlgorithm);
            l.A("relevant platform keys hashed");

            HashRelevantFiles(files, hashAlgorithm);
            l.A("relevant files hashed");

            hashAlgorithm.TransformFinalBlock(new byte[0], 0, 0); // Finalize the hash computation
            l.A("finalized hash");

            return l.ReturnAsOk(hashAlgorithm.Hash);
        }

        /// <summary>
        /// Hash relevant keys
        /// </summary>
        /// <param name="hashAlgorithm"></param>
        private void HashRelevantKeys(HashAlgorithm hashAlgorithm)
            => hashAlgorithm.TransformBlock(PlatformBytes, 0, PlatformBytes.Length, PlatformBytes, 0);

        // Cache platform bytes
        private byte[] PlatformBytes => _platformBytes ??= GetPlatformBytes();
        private static byte[] _platformBytes;

        // Combines relevant keys, incl. 2sxc and platform versions to hash.
        private byte[] GetPlatformBytes()
        {
            var l = Log.Fn<byte[]>($"", timer: true);
            var platformVersion = platform.Value.Version;
            var sxcVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var platformString = $"{platform.Value.Name.ToLowerInvariant()}:{platformVersion.Major}.{platformVersion.Minor}.{platformVersion.Build}.{platformVersion.Revision}_2sxc:{sxcVersion.Major}.{sxcVersion.Minor}.{sxcVersion.Build}.{sxcVersion.Revision}";
            l.A($"platform string: '{platformString}'");
            return l.ReturnAsOk(System.Text.Encoding.UTF8.GetBytes(platformString));
        }

        /// <summary>
        /// Hash relevant files in the folder.
        /// </summary>
        /// <param name="files"></param>
        /// <param name="hashAlgorithm"></param>
        private static void HashRelevantFiles(string[] files, HashAlgorithm hashAlgorithm)
        {
            // Hash all files in the folder
            foreach (var filePath in files)
            {
                // Read and hash the file content
                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var buffer = new byte[BufferSize];
                int bytesRead;
                while ((bytesRead = fs.Read(buffer, 0, BufferSize)) > 0)
                    hashAlgorithm.TransformBlock(buffer, 0, bytesRead, buffer, 0);
            }
        }
    }
}
