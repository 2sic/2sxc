using System.Linq;
using System.Threading;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// Helpers to get the metadata for ADAM items
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// Find the first metadata entity for this file/folder
        /// </summary>
        /// <param name="app">the app which manages the metadata</param>
        /// <param name="mdId"></param>
        /// <returns></returns>
        internal static IEntity GetFirstMetadata(AppRuntime app, MetadataFor mdId)
            => app.Metadata
                .Get(mdId.TargetType, mdId.KeyString) //(isFolder ? "folder:" : "file:") + id)
                .FirstOrDefault();

        /// <summary>
        /// Get the first metadata entity of an item - or return a fake one instead
        /// </summary>
        internal static IDynamicEntity GetFirstOrFake(AdamAppContext appContext, MetadataFor mdId)
        {
            var meta = GetFirstMetadata(appContext.AppRuntime, mdId) 
                       ?? Build.FakeEntity(Eav.Constants.TransientAppId);
            return new DynamicEntity(meta, 
                new[] { Thread.CurrentThread.CurrentCulture.Name }, 
                appContext.CompatibilityLevel, 
                appContext.Block);
        }

    }
}
