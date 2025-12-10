using System.Net.Http;
using ToSic.Eav.Security.Files;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.App;

/// <summary>
/// Downloads extension packages from remote URLs and returns a stream plus a sanitized filename.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionDownloadBackend() : ServiceBase("Bck.ExtDl")
{
    public (Stream Stream, string FileName, string Url) DownloadFirstAvailable(string[] urls)
    {
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        var l = Log.Fn<(Stream, string, string)>($"urls:{urls?.Length ?? 0}");

        if (urls == null || urls.Length == 0)
            throw l.Ex(new ArgumentException("no urls provided"));

        var candidates = urls.Where(u => u.HasValue()).ToList();
        if (candidates == null || candidates.Count == 0)
            throw l.Ex(new ArgumentException("no valid urls provided"));

        l.A($"download candidates:{candidates.Count}");

        Exception? lastError = null;
        foreach (var url in candidates)
        {
            try
            {
                l.A($"try download url:'{url}'");
                var result = DownloadSingleUrl(url);
                return l.Return((result.Stream, result.FileName, url), $"ok '{url}'");
            }
            catch (Exception ex)
            {
                lastError = ex;
                l.A($"download failed for '{url}': {ex.Message}");
            }
        }

        throw l.Ex(new InvalidOperationException("Could not download extension package from provided URLs.", lastError));
    }

    private (Stream Stream, string FileName) DownloadSingleUrl(string url)
    {
        var l = Log.Fn<(Stream, string)>($"url:'{url}'");

        MemoryStream? stream = null;
        try
        {
            var fileName = BuildFileName(url);

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "2sxc-extension-installer");
            httpClient.Timeout = TimeSpan.FromMinutes(5);
            using var response = httpClient.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();

            stream = new MemoryStream();
            response.Content.CopyToAsync(stream).Wait();
            stream.Position = 0;

            return l.ReturnAsOk((stream, fileName));
        }
        catch (HttpRequestException ex)
        {
            stream?.Dispose();
            var wrapped = new InvalidOperationException($"Could not download extension package from '{url}'.", ex);
            l.Ex(wrapped);
            throw wrapped;
        }
        catch (Exception ex)
        {
            stream?.Dispose();
            l.Ex(ex);
            throw;
        }
    }

    private static string BuildFileName(string url)
    {
        const string defaultName = "extension-remote.zip";

        var fileName = defaultName;
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            var candidate = Path.GetFileName(uri.LocalPath);
            if (candidate.HasValue())
                fileName = FileNames.SanitizeFileName(candidate);
        }

        if (!fileName.HasValue())
            fileName = defaultName;

        if (FileNames.IsKnownRiskyExtension(fileName))
            throw new ArgumentException($"File {fileName} has risky file type.");

        return fileName;
    }
}
