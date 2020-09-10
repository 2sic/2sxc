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
        /// <param name="id">the id of the file/folder</param>
        /// <param name="isFolder">if it's a file or a folder</param>
        /// <returns></returns>
        internal static IEntity GetFirstMetadata(AppRuntime app, int id, bool isFolder)
            => app.Metadata
                .Get(Eav.Constants.MetadataForCmsObject,
                    (isFolder ? "folder:" : "file:") + id)
                .FirstOrDefault();

        /// <summary>
        /// Get the first metadata entity of an item - or return a fake one instead
        /// </summary>
        internal static IDynamicEntity GetFirstOrFake(AdamAppContext appContext, int id, bool isFolder)
        {
            var meta = GetFirstMetadata(appContext.AppRuntime, id, isFolder) 
                       ?? Build.FakeEntity(Eav.Constants.TransientAppId);
            return new DynamicEntity(meta, 
                new[] { Thread.CurrentThread.CurrentCulture.Name }, 
                appContext.CompatibilityLevel, 
                appContext.Block);
        }

        //public static int GetMetadataId(AppRuntime appRuntime, int id, bool isFolder)
        //    => GetFirstMetadata(appRuntime, id, isFolder)?.EntityId ?? 0;


    }
}
