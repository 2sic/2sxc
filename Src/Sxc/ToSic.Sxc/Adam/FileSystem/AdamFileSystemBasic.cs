using ToSic.Lib.Logging;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// Basic implementation of the ADAM file system.
    /// This is string-based, not with environment IDs.
    /// It's primarily meant for standalone implementations or as a template for other integrations. 
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public partial class AdamFileSystemBasic: AdamFileSystemBasic<string, string>, IAdamFileSystem<string, string>
    {
        #region Constructor / DI / Init

        public AdamFileSystemBasic(IAdamPaths adamPaths) : base(adamPaths, LogScopes.Base)
        {
        }

        #endregion

        
    }
}
