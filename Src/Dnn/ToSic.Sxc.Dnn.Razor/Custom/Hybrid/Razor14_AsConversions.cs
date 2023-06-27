using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Sxc.Adam;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Razor14
    {
        #region AsDynamic in many variations

        /// <inheritdoc/>
        public dynamic AsDynamic(string json, string fallback = default) => _DynCodeRoot.AsC.AsDynamicFromJson(json, fallback);

        /// <inheritdoc/>
        public dynamic AsDynamic(IEntity entity) => _DynCodeRoot.AsC.AsDynamic(entity);

        /// <inheritdoc/>
        public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot.AsDynamic(dynamicEntity);

        /// <inheritdoc/>
        public dynamic AsDynamic(params object[] entities) => _DynCodeRoot.AsC.MergeDynamic(entities);

        #endregion

        #region AsEntity
        /// <inheritdoc/>
        public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot.AsC.AsEntity(dynamicEntity);
        #endregion

        #region AsList

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(object list) => _DynCodeRoot.AsC.AsDynamicList(list);

        #endregion

        /// <inheritdoc />
        public IFolder AsAdam(ICanBeEntity item, string fieldName) => _DynCodeRoot.AsAdam(item, fieldName);

    }


}
