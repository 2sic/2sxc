using ToSic.Eav.Documentation;
using ToSic.Sxc.Interfaces;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// Describes an ADAM asset.
    /// </summary>
    [PublicApi]
    internal interface IAsset
    {
        #region Metadata
        /// <summary>
        /// True if this asset has metadata
        /// </summary>
        bool HasMetadata { get; }

        /// <summary>
        /// List of metadata items - 
        /// will automatically contain a fake item, even if no metadata exits
        /// to help in razor template etc.
        /// </summary>
        IDynamicEntity Metadata { get; }
        #endregion


        /// <summary>
        /// The path to this asset as used from external access
        /// </summary>
        string Url { get; }

        /// <summary>
        /// The type of this asset (folder, file, etc.)
        /// </summary>
        string Type { get; }

        /// <summary>
        /// The asset name
        /// typically the folder or teh file name
        /// </summary>
        string Name { get; }
    };
}

