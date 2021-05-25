using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Compatibility;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public partial class Api12: IDynamicCodeBeforeV10
    {
        // Obsolete stuff - not supported any more in after V10 - show helpful error messages

        private const string NotSupportedIn10 = "is not supported in new Api Controllers";
        
        #region Shared Code Block between RazorComponent_Obsolete and ApiController_Obsolete

        #region Obsolete CreateSource

        [PrivateApi]
        [Obsolete("for compatibility only, will throw error with instructions how to fix. Use CreateSource<type> instead.")]
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, ILookUpEngine lookUpEngine = null)
            => throw new Exception($"CreateSource(string, ...) {NotSupportedIn10}. Please use CreateSource<DataSourceTypeName>(...) instead.");

        #endregion

        #region Compatibility with Eav.Interfaces.IEntity - introduced in 10.10

        [PrivateApi]
        [Obsolete("for compatibility only, will throw error with instructions how to fix. Cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(ToSic.Eav.Interfaces.IEntity entity)
            => throw new Exception($"AsDynamic(Eav.Interfaces.IEntity) {NotSupportedIn10}. Please cast your data to ToSic.Eav.Data.IEntity.");


        [PrivateApi]
        [Obsolete("for compatibility only, will throw error with instructions how to fix. Cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(KeyValuePair<int, ToSic.Eav.Interfaces.IEntity> entityKeyValuePair)
            => throw new Exception($"AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity>) {NotSupportedIn10}. Please cast your data to ToSic.Eav.Data.IEntity.");

        [PrivateApi]
        [Obsolete("for compatibility only, will throw error with instructions how to fix. Cast your entities to ToSic.Eav.Data.IEntity")]
        public IEnumerable<dynamic> AsDynamic(IEnumerable<ToSic.Eav.Interfaces.IEntity> entities)
            => throw new Exception($"AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities) {NotSupportedIn10}. Please cast your data to ToSic.Eav.Data.IEntity.");

        #endregion

        #region AsDynamic<int, IEntity>

        [PrivateApi]
        [Obsolete("for compatibility only, will throw error with instructions how to fix. Use AsDynamic(IEnumerable<IEntity>...)")]
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair)
            => throw new Exception($"AsDynamic(KeyValuePair<int, IEntity> {NotSupportedIn10}. Use AsDynamic(IEnumerable<IEntity>...).");

        #region Old AsDynamic with correct warnings
        /// <inheritdoc/>
        [PrivateApi]
        public IEnumerable<dynamic> AsDynamic(IDataStream stream)
            => throw new Exception($"AsDynamic for lists isn't supported here. Please use AsList(...) instead.");

        /// <inheritdoc/>
        [PrivateApi]
        public IEnumerable<dynamic> AsDynamic(IDataSource source)
            => throw new Exception($"AsDynamic for lists isn't supported here. Please use AsList(...) instead.");


        /// <inheritdoc/>
        [PrivateApi]
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities)
            => throw new Exception($"AsDynamic for lists isn't supported here. Please use AsList(...) instead.");

        #endregion
        #endregion

        #region Presentation, ListContent, ListPresentation, List

        [PrivateApi]
        [Obsolete("use Content.Presentation instead")]
        public dynamic Presentation
            => throw new Exception($"Presentation {NotSupportedIn10}. Use Content.Presentation.");


        [PrivateApi]
        [Obsolete("Use Header instead")]
        public dynamic ListContent
            => throw new Exception($"ListContent {NotSupportedIn10}. Use Header.");

        [PrivateApi]
        [Obsolete("Use Header.Presentation instead")]
        public dynamic ListPresentation
            => throw new Exception($"ListPresentation {NotSupportedIn10}. Use Header.Presentation");

        [PrivateApi]
        [Obsolete("This is an old way used to loop things - removed in RazorComponent")]
        public object List
            => throw new Exception($"List {NotSupportedIn10}. Use Data[\"DefaultAuthenticationEventArgs\"].List");

        #endregion

        #endregion

    }
}
