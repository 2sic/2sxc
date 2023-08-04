using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Razor14
    {
        #region AsDynamic in many variations

        /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
        public dynamic AsDynamic(string json, string fallback = default) => _DynCodeRoot.AsC.AsDynamicFromJson(json, fallback);

        /// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
        public dynamic AsDynamic(IEntity entity) => _DynCodeRoot.AsC.CodeAsDyn(entity);

        /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
        public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot.AsC.AsDynamicFromObject(dynamicEntity);

        /// <inheritdoc cref="IDynamicCode12.AsDynamic(object[])" />
        public dynamic AsDynamic(params object[] entities) => _DynCodeRoot.AsC.MergeDynamic(entities);

        #endregion

        #region AsEntity
        /// <inheritdoc cref="IDynamicCode.AsEntity" />
        public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot.AsC.AsEntity(dynamicEntity);
        #endregion

        #region AsList

        /// <inheritdoc cref="IDynamicCode.AsList" />
        public IEnumerable<dynamic> AsList(object list) => _DynCodeRoot.AsC.CodeAsDynList(list);

        #endregion

        /// <inheritdoc cref="IDynamicCode.AsAdam" />
        public IFolder AsAdam(ICanBeEntity item, string fieldName) => _DynCodeRoot.AsAdam(item, fieldName);

    }


}
