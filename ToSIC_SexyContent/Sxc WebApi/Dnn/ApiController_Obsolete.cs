using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Dnn
{
    public partial class ApiController
    {
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null,
            ITokenListFiller configurationProvider = null)
            => DynCodeHelpers.CreateSource(typeName, inSource, configurationProvider);

        /// <summary>
        /// presentation item of the content-item. 
        /// </summary>
        [Obsolete("please use Content.Presentation instead")]
        public dynamic Presentation => DynCodeHelpers.Content?.Presentation;

        [Obsolete("use Header instead")]
        public dynamic ListContent => DynCodeHelpers.Header;

        /// <summary>
        /// presentation item of the content-item. 
        /// </summary>
        [Obsolete("please use Header.Presentation instead")]
        public dynamic ListPresentation => DynCodeHelpers.Header?.Presentation;

        [Obsolete("This is an old way used to loop things. Use Data[\"Default\"] instead. Will be removed in 2sxc v10")]
        public object List => DynCodeHelpers.List;



        #region Compatibility with Eav.Interfaces.IEntity - introduced in 10.10
        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(Eav.Interfaces.IEntity entity) => DynCodeHelpers.AsDynamic(entity);


        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity> entityKeyValuePair) => DynCodeHelpers.AsDynamic(entityKeyValuePair.Value);

        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public IEnumerable<dynamic> AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities) => DynCodeHelpers.AsDynamic(entities);
        #endregion



        //// todo: only in "old" controller, not in new one
        ///// <inheritdoc />
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) => DynCodeHelpers.AsDynamic(entityKeyValuePair.Value);

    }
}
