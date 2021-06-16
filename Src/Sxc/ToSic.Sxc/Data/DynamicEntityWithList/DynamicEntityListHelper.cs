using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is a helper in charge of the list-behavior of a DynamicEntity
    /// </summary>
    [PrivateApi]
    internal class DynamicEntityListHelper
    {
        public readonly IEntity Parent;
        public readonly string Field;
        private readonly DynamicEntityDependencies _dependencies;

        public DynamicEntityListHelper(IDynamicEntity singleItem, DynamicEntityDependencies dependencies)
        {
            _list = new List<IDynamicEntity> {singleItem ?? throw new ArgumentException(nameof(singleItem))};
            _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));

        }
        
        public DynamicEntityListHelper(IEntity parent, string field, IEnumerable<IEntity> entities, DynamicEntityDependencies dependencies)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            Field = field ?? throw new ArgumentNullException(nameof(field));
            _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
            _entities = entities?.ToArray() ?? throw new ArgumentNullException(nameof(entities));
        }
        
        private List<IDynamicEntity> _list;
        private readonly IEntity[] _entities;

        [PrivateApi]
        public List<IDynamicEntity> DynEntities
        {
            get
            {
                // Case #1 & Case #2- already created before or because of Single-Item
                if (_list != null) return _list;
                
                // Case #3 - Real sub-list
                var index = 0;
                return _list = _entities
                    .Select(e =>
                    {
                        // we create an Entity with some metadata-decoration, so that toolbars know it's part of a list
                        var blockEntity = new EntityInBlock(e, Parent.EntityGuid, Field, index++);
                        return DynamicEntityBase.SubDynEntity(blockEntity, _dependencies);
                    })
                    .ToList();
            }
        }

        public int Count => _entities?.Length ?? 1;


    }
}
