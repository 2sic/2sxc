using System;
using System.Collections.Generic;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    public abstract class DynamicEntityBase: DynamicObject
    {
        protected DynamicEntityBase(DynamicEntityDependencies dependencies) => _Dependencies = dependencies;

        [PrivateApi("Try to replace existing list of shared properties")]
        // ReSharper disable once InconsistentNaming
        public DynamicEntityDependencies _Dependencies { get; }

        protected readonly Dictionary<string, object> _ValueCache = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);


        protected object ValueAutoConverted(object result, string dataType, bool lookup, IEntity entity, string field)
        {
            // New mechanism to not use resolve-hyperlink
            if (lookup && result is string strResult
                       && dataType == DataTypes.Hyperlink
                       && ValueConverterBase.CouldBeReference(strResult))
                result = _Dependencies.ValueConverterOrNull?.ToValue(strResult, entity.EntityGuid) ?? result;

            // note 2021-06-07 previously in created sub-entities with modified language-list; I think this is wrong
            return result is IEnumerable<IEntity> children
                ? new DynamicEntityWithList(entity, field, children, _Dependencies)
                : result;
        }

    }
}
