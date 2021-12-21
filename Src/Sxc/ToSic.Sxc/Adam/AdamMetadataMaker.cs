using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// Helpers to get the metadata for ADAM items
    /// </summary>
    public class AdamMetadataMaker
    {
        public AdamMetadataMaker(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Find the first metadata entity for this file/folder
        /// </summary>
        /// <param name="app">the app which manages the metadata</param>
        /// <param name="mdId"></param>
        /// <returns></returns>
        internal IEnumerable<IEntity> GetMetadata(AppRuntime app, ITarget mdId)
            => app.Metadata.Get(mdId.TargetType, mdId.KeyString);

        /// <summary>
        /// Get the first metadata entity of an item - or return a fake one instead
        /// </summary>
        internal IDynamicMetadata GetMetadata(AdamManager manager, string key, string title)
        {
            var mdOf = new MetadataOf<string>((int)TargetTypes.CmsItem, key, manager.AppRuntime.AppState, title);
            return new DynamicMetadata(mdOf, null, DynamicEntityDependencies(manager));
        }

        private DynamicEntityDependencies DynamicEntityDependencies(AdamManager manager) =>
            _dynamicEntityDependencies
            ?? (_dynamicEntityDependencies = _serviceProvider.Build<DynamicEntityDependencies>()
                .Init(null, (manager.AppContext?.Site).SafeLanguagePriorityCodes(), null, manager.CompatibilityLevel));
        private DynamicEntityDependencies _dynamicEntityDependencies;


    }
}
