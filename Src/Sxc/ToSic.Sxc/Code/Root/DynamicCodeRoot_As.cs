using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data.AsConverter;
using ToSic.Sxc.Services;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {
        public AsConverterService AsC => _asc.Get(() =>
        {
            Services.AsConverter.ConnectToRoot(this);
            return Services.AsConverter;
        });
        private readonly GetOnce<AsConverterService> _asc = new GetOnce<AsConverterService>();

        #region AsDynamic Implementations

        /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
        public dynamic AsDynamic(string json, string fallback = default) => AsC.AsDynamicFromJson(json, fallback);

        /// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
        public dynamic AsDynamic(IEntity entity) => AsC.AsDynamic(entity);

        /// <inheritdoc cref="IDynamicCode.AsDynamic(object)" />
        public dynamic AsDynamic(object dynamicEntity) => AsC.AsDynamicInternal(dynamicEntity);

        /// <inheritdoc cref="IDynamicCode12.AsDynamic(object[])" />
        public dynamic AsDynamic(params object[] entities) => AsC.MergeDynamic(entities);


        /// <inheritdoc cref="IDynamicCode.AsEntity" />
        public IEntity AsEntity(object dynamicEntity) => AsC.AsEntity(dynamicEntity);

        #endregion

        #region AsList

        /// <inheritdoc cref="IDynamicCode.AsList" />
        public IEnumerable<dynamic> AsList(object list) => AsC.AsDynamicList(list);

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
