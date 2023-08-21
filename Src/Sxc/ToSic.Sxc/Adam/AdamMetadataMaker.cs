using System;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// Helpers to get the metadata for ADAM items
    /// </summary>
    public class AdamMetadataMaker
    {

        /// <summary>
        /// Get the first metadata entity of an item - or return a fake one instead
        /// </summary>
        internal static IMetadata Create(AdamManager manager, string key, string title, Action<IMetadataOf> mdInit = null)
        {
            var mdOf = new MetadataOf<string>((int)TargetTypes.CmsItem, key, title, null, manager.AppRuntime.AppState);
            mdInit?.Invoke(mdOf);
            return manager.Cdf.Metadata(mdOf);
        }
    }
}
