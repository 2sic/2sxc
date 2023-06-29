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

        /// <inheritdoc />
        public dynamic AsDynamic(string json, string fallback = default) => AsC.AsDynamicFromJson(json, fallback);

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => AsC.AsDynamic(entity);

        /// <inheritdoc />
        public dynamic AsDynamic(object dynamicEntity) => AsC.AsDynamicInternal(dynamicEntity);

        /// <inheritdoc />
        [PublicApi]
        public dynamic AsDynamic(params object[] entities) => AsC.MergeDynamic(entities);


        /// <inheritdoc />
        public IEntity AsEntity(object dynamicEntity) => AsC.AsEntity(dynamicEntity);

        #endregion

        #region AsList

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(object list) => AsC.AsDynamicList(list);

        #endregion

        #region Convert

        /// <inheritdoc />
        public IConvertService Convert => _convert ?? (_convert = Services.ConvertService.Value);
        private IConvertService _convert;

        #endregion

        #region Adam

        /// <inheritdoc />
        public IFolder AsAdam(ICanBeEntity item, string fieldName) => AsC.Folder(item, fieldName);

        #endregion
    }
}
