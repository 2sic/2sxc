using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Builder;
using ToSic.SexyContent;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// Helpers to get the metadata for ADAM items
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// Generate an empty, fake metadata item
        /// Important so that all ADAM items have a metadata - and don't throw errors when accessed
        /// </summary>
        /// <returns></returns>
        internal static Entity CreateFakeMetadata()
        {
            var emptyMetadata = new Dictionary<string, object> { { "Title", "" } };
            var fakeMeta = new Entity(Eav.Constants.TransientAppId, 0, ContentTypeBuilder.Fake(""), emptyMetadata,
                "Title");
            return fakeMeta;
        }

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
            var meta = GetFirstMetadata(appContext.AppRuntime, id, isFolder) ?? CreateFakeMetadata();
            return new DynamicEntity(meta, new[] { Thread.CurrentThread.CurrentCulture.Name }, appContext.CompatibilityLevel, appContext.BlockBuilder);
        }

        public static int GetMetadataId(AppRuntime appRuntime, int id, bool isFolder)
            => GetFirstMetadata(appRuntime, id, isFolder)?.EntityId ?? 0;


    }
}
