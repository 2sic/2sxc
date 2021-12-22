﻿using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;

 // ReSharper disable once CheckNamespace
 namespace Custom.Hybrid
{
    // Important Note
    // The new hybrid implementation doesn't actually need this
    // But if we add these overloads in an inherited class, they will be preferred to the real working ones
    // which would result in errors on AsDynamic(some-object) even though it should just work
    public partial class Razor12
    {
        // Obsolete stuff - not supported any more in RazorPage10 - maybe re-activate to show helpful error messages

        #region Shared Code Block between RazorComponent_Obsolete and ApiController_Obsolete

        private const string NotSupportedIn10 = "is not supported in Razor12 or RazorComponent (v10)";

        #region Obsolete CreateSource

        [PrivateApi]
        [Obsolete("for compatibility only, will throw error with instructions how to fix. Use CreateSource<type> instead.")]
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, ILookUpEngine lookUpEngine = null)
            => throw new Exception($"CreateSource(string, ...) {NotSupportedIn10}. Please use CreateSource<DataSourceTypeName>(...) instead.");

        #endregion

        #region Compatibility with Eav.Interfaces.IEntity - introduced in 10.10

        [Obsolete("for compatibility only, will throw error with instructions how to fix. Cast your entities to ToSic.Eav.Data.IEntity")]
        [PrivateApi]
        public dynamic AsDynamic(ToSic.Eav.Interfaces.IEntity entity)
            => throw new Exception($"AsDynamic(Eav.Interfaces.IEntity) {NotSupportedIn10}. Please cast your data to ToSic.Eav.Data.IEntity.");


        [Obsolete("for compatibility only, will throw error with instructions how to fix. Cast your entities to ToSic.Eav.Data.IEntity")]
        [PrivateApi]
        public dynamic AsDynamic(KeyValuePair<int, ToSic.Eav.Interfaces.IEntity> entityKeyValuePair)
            => throw new Exception($"AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity>) {NotSupportedIn10}. Please cast your data to ToSic.Eav.Data.IEntity.");

        [Obsolete("for compatibility only, will throw error with instructions how to fix. Cast your entities to ToSic.Eav.Data.IEntity")]
        [PrivateApi]
        public IEnumerable<dynamic> AsDynamic(IEnumerable<ToSic.Eav.Interfaces.IEntity> entities)
            => throw new Exception($"AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities) {NotSupportedIn10}. Please cast your data to ToSic.Eav.Data.IEntity.");

        #endregion

        #region AsDynamic<int, IEntity>

        [PrivateApi]
        [Obsolete("for compatibility only, will throw error with instructions how to fix. Use AsDynamic(IEnumerable<IEntity>...)")]
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair)
            => throw new Exception($"AsDynamic(KeyValuePair<int, IEntity> {NotSupportedIn10}. Use AsDynamic(IEnumerable<IEntity>...).");

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
        public IEnumerable<dynamic> List
            => throw new Exception($"List {NotSupportedIn10}. Use Data[\"DefaultAuthenticationEventArgs\"].List");

        #endregion

        #endregion

        //[PrivateApi("this is the old signature, should still be supported")]
        // we're not creating an error/overload here, because it may lead to signature issues 
        // where two methods have almost the same signature
        //public virtual void CustomizeSearch(Dictionary<string, List<ISearchInfo>> searchInfos, ModuleInfo moduleInfo, DateTime beginDate)
        //    => throw new Exception($"ListPresentation {NotSupportedIn10}. Use Header.Presentation");

        #region Old AsDynamic with correct warnings
        /// <inheritdoc/>
        [PrivateApi]
        public IEnumerable<dynamic> AsDynamic(IDataStream stream)
            => throw new Exception($"AsDynamic for lists isn't supported in RazorComponent. Please use AsList instead.");

        /// <inheritdoc/>
        [PrivateApi]
        public IEnumerable<dynamic> AsDynamic(IDataSource source)
            => throw new Exception($"AsDynamic for lists isn't supported in RazorComponent. Please use AsList instead.");


        /// <inheritdoc/>
        [PrivateApi]
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities)
            => throw new Exception($"AsDynamic for lists isn't supported in RazorComponent. Please use AsList instead.");

        #endregion

    }
}