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


        protected object ValueAutoConverted(object result, string dataType, bool lookup, IEntity entity, string field)
        {
            // New mechanism to not use resolve-hyperlink
            if (lookup && result is string strResult
                       && dataType == DataTypes.Hyperlink
                       && ValueConverterBase.CouldBeReference(strResult))
                result = _Dependencies.ValueConverterOrNull?.ToValue(strResult, entity.EntityGuid) ?? result;

            if (result is IEnumerable<IEntity> children)
                // note 2021-06-07 previously in created sub-entities with modified language-list; I think this is wrong
                result = new DynamicEntityWithList(entity, field, children, _Dependencies);

            return result;
        }

    }
}
