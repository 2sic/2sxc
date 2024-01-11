namespace ToSic.Sxc.Adam.Internal;

/// <summary>
/// Basic implementation of the ADAM file system.
/// This is string-based, not with environment IDs.
/// It's primarily meant for standalone implementations or as a template for other integrations. 
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public partial class AdamFileSystemBasic(IAdamPaths adamPaths)
    : AdamFileSystemBasic<string, string>(adamPaths, LogScopes.Base), IAdamFileSystem<string, string>;