using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {
        public CodeDataFactory AsC => _asc.Get(() =>
        {
            Services.AsConverter.ConnectToRoot(this);
            return Services.AsConverter;
        });
        private readonly GetOnce<CodeDataFactory> _asc = new GetOnce<CodeDataFactory>();

        #region AsDynamic Implementations

        /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
        public dynamic AsDynamic(string json, string fallback = default) => AsC.AsDynamicFromJson(json, fallback);

        /// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
        public dynamic AsDynamic(IEntity entity) => AsC.CodeAsDyn(entity);

        /// <inheritdoc cref="IDynamicCode.AsDynamic(object)" />
        public dynamic AsDynamic(object dynamicEntity) => AsC.AsDynamicFromObject(dynamicEntity);

        /// <inheritdoc cref="IDynamicCode12.AsDynamic(object[])" />
        public dynamic AsDynamic(params object[] entities) => AsC.MergeDynamic(entities);


        /// <inheritdoc cref="IDynamicCode.AsEntity" />
        public IEntity AsEntity(object dynamicEntity) => AsC.AsEntity(dynamicEntity);

        #endregion

        #region AsList

        /// <inheritdoc cref="IDynamicCode.AsList" />
        public IEnumerable<dynamic> AsList(object list) => AsC.CodeAsDynList(list);

        #endregion

        #region Convert

        /// <inheritdoc />
        public IConvertService Convert => _convert ?? (_convert = Services.ConvertService.Value);
        private IConvertService _convert;

        #endregion

        #region Adam

        /// <inheritdoc cref="IDynamicCode.AsAdam" />
        public IFolder AsAdam(ICanBeEntity item, string fieldName) => AsC.Folder(item, fieldName);

        #endregion
    }
}
