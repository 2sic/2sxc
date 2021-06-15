using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {

        #region AsDynamic Implementations

        /// <inheritdoc />
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => DynamicJacket.AsDynamicJacket(json, fallback);

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => new DynamicEntity(entity, DynamicEntityDependencies);

        private DynamicEntityDependencies DynamicEntityDependencies =>
            _dynamicEntityDependencies
            ?? (_dynamicEntityDependencies = new DynamicEntityDependencies(Block, _serviceProvider,
                CmsContext.SafeLanguagePriorityCodes(), Log, CompatibilityLevel));
        private DynamicEntityDependencies _dynamicEntityDependencies;

        /// <inheritdoc />
        public dynamic AsDynamic(object dynamicEntity) => dynamicEntity;

        /// <inheritdoc />
        [PublicApi("Careful - still Experimental in 12.02")]
        public dynamic AsDynamic(params object[] entities)
        {
            if (entities == null || !entities.Any()) return null;
            if (entities.Length == 1)
            {
                var first = entities[0];
                if (first is IDynamicEntity dynEntity) return dynEntity;
                if (first is IEntity entity) return new DynamicEntity(entity, DynamicEntityDependencies);
                // Unknown, just return it and hope for the best
                // probably not a good solution
                return first;
            }
            // New case: many items found, must create a stack
            var sources = entities
                .Select(e => e as IPropertyLookup)
                .Where(e => e !=null)
                .Select(e => new KeyValuePair<string, IPropertyLookup>(null, e));
            return new DynamicStack(DynamicEntityDependencies, sources.ToArray());
        }


        /// <inheritdoc />
        public IEntity AsEntity(object dynamicEntity) => ((IDynamicEntity)dynamicEntity).Entity;

        #endregion

        #region AsList

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(object list)
        {
            switch (list)
            {
                case null:
                    return new List<dynamic>();
                case IDataSource dsEntities:
                    return AsList(dsEntities[Eav.Constants.DefaultStreamName]);
                case IEnumerable<IEntity> iEntities:
                    return iEntities.Select(e => AsDynamic(e));
                case IEnumerable<dynamic> dynEntities:
                    return dynEntities;
                default:
                    return null;
            }
        }

        #endregion


        #region Adam

        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName)
            => AsAdam(AsEntity(entity), fieldName);


        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName)
        {
            if (_adamManager == null)
                _adamManager = GetService<AdamManager>()
                    .Init(Block.Context, CompatibilityLevel, Log);
            return _adamManager.Folder(entity, fieldName);
        }
        private AdamManager _adamManager;

        #endregion
    }
}
