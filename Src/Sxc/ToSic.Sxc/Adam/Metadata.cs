using System;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// Helpers to get the metadata for ADAM items
    /// </summary>
    public class AdamMetadataMaker
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Lazy<IDataBuilder> _dataBuilderLazy;

        public AdamMetadataMaker(IServiceProvider serviceProvider, Lazy<IDataBuilder> dataBuilderLazy)
        {
            _serviceProvider = serviceProvider;
            _dataBuilderLazy = dataBuilderLazy;
        }

        /// <summary>
        /// Find the first metadata entity for this file/folder
        /// </summary>
        /// <param name="app">the app which manages the metadata</param>
        /// <param name="mdId"></param>
        /// <returns></returns>
        internal IEntity GetFirstMetadata(AppRuntime app, MetadataFor mdId)
            => app.Metadata
                .Get(mdId.TargetType, mdId.KeyString)
                .FirstOrDefault();

        /// <summary>
        /// Get the first metadata entity of an item - or return a fake one instead
        /// </summary>
        internal IDynamicEntity GetFirstOrFake(AdamManager manager, MetadataFor mdId)
        {
            var meta = GetFirstMetadata(manager.AppRuntime, mdId) 
                       ?? _dataBuilderLazy.Value.FakeEntity(Eav.Constants.TransientAppId);
            //var dynEnt = new DynamicEntity(meta,
            //    (manager.AppContext?.Site).SafeLanguagePriorityCodes(),
            //    manager.CompatibilityLevel,
            //    null, _serviceProvider);
            var dynEnt = new DynamicEntity(meta, DynamicEntityDependencies(manager));
            return dynEnt;
        }

        private DynamicEntityDependencies DynamicEntityDependencies(AdamManager manager) =>
            _dynamicEntityDependencies
            ?? (_dynamicEntityDependencies = new DynamicEntityDependencies(null, _serviceProvider,
                (manager.AppContext?.Site).SafeLanguagePriorityCodes(), null, manager.CompatibilityLevel));
        private DynamicEntityDependencies _dynamicEntityDependencies;


    }
}
