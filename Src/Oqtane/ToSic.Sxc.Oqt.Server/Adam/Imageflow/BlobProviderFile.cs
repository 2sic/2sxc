using System;
using System.IO;
using Imazen.Common.Storage;

namespace ToSic.Sxc.Oqt.Server.Adam.Imageflow;
#pragma warning disable S3881 // "IDisposable" should be implemented correctly
internal class BlobProviderFile : IBlobData
#pragma warning restore S3881 // "IDisposable" should be implemented correctly
{
    public string Path;
    public bool? Exists { get; set; }
    public DateTime? LastModifiedDateUtc { get; set; }
    public Stream OpenRead()
    {
        return new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
    }

    public void Dispose()
    {
        // Method intentionally left empty.
    }
}