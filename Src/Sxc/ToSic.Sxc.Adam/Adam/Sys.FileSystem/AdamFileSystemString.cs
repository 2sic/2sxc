using ToSic.Sxc.Adam.Paths.Internal;

namespace ToSic.Sxc.Adam.Sys.FileSystem;

/// <summary>
/// Basic implementation of the ADAM file system.
/// This is string-based, not with environment IDs.
/// It's primarily meant for standalone implementations or as a template for other integrations. 
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class AdamFileSystemString(IAdamPaths adamPaths)
    : AdamFileSystemBase(adamPaths, LogScopes.Base, []), IAdamFileSystem;