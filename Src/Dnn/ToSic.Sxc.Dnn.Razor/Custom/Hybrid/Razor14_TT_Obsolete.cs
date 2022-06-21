﻿using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
 using ToSic.Sxc.Custom.Hybrid;

 // ReSharper disable once CheckNamespace
 namespace Custom.Hybrid
{
    // Important Note
    // The new hybrid implementation doesn't actually need this
    // But if we add these overloads in an inherited class, they will be preferred to the real working ones
    // which would result in errors on AsDynamic(some-object) even though it should just work
    public abstract partial class Razor14<TModel, TServiceKit>
    {
        // Obsolete stuff - not supported any more in RazorPage10 - maybe re-activate to show helpful error messages

        #region Shared Code Block between RazorComponent_Obsolete and ApiController_Obsolete

        #region Obsolete CreateSource

        [PrivateApi]
        [Obsolete("throws error with fix-instructions. Use CreateSource<type> instead.")]
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, ILookUpEngine lookUpEngine = null)
            => Obsolete10.CreateSourceString();

        #endregion

        #region Compatibility with Eav.Interfaces.IEntity - introduced in 10.10

        [PrivateApi]
        [Obsolete("throws error with fix-instructions. Cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(ToSic.Eav.Interfaces.IEntity entity)
            => Obsolete10.AsDynamicInterfacesIEntity();


        [PrivateApi]
        [Obsolete("throws error with fix-instructions. Cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(KeyValuePair<int, ToSic.Eav.Interfaces.IEntity> entityKeyValuePair)
            => Obsolete10.AsDynamicKvpInterfacesIEntity();

        [Obsolete("throws error with fix-instructions. Cast your entities to ToSic.Eav.Data.IEntity")]
        [PrivateApi]
        public IEnumerable<dynamic> AsDynamic(IEnumerable<ToSic.Eav.Interfaces.IEntity> entities)
            => Obsolete10.AsDynamicIEnumInterfacesIEntity();

        #endregion

        #region AsDynamic<int, IEntity>

        [PrivateApi]
        [Obsolete("throws error with fix-instructions. Use AsDynamic(IEnumerable<IEntity>...)")]
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) => Obsolete10.AsDynamicKvp();

        #endregion

        #region Presentation, ListContent, ListPresentation, List

        [PrivateApi]
        [Obsolete("use Content.Presentation instead")]
        public dynamic Presentation => Obsolete10.Presentation();


        [PrivateApi]
        [Obsolete("Use Header instead")]
        public dynamic ListContent => Obsolete10.ListContent();

        [PrivateApi]
        [Obsolete("Use Header.Presentation instead")]
        public dynamic ListPresentation => Obsolete10.ListPresentation();

        [PrivateApi]
        [Obsolete("This is an old way used to loop things - removed in RazorComponent")]
        public IEnumerable<dynamic> List => Obsolete10.List();

        #endregion

        #endregion

        #region Old AsDynamic with correct warnings

        [PrivateApi] public IEnumerable<dynamic> AsDynamic(IDataStream stream) => Obsolete10.AsDynamicForList();
        [PrivateApi] public IEnumerable<dynamic> AsDynamic(IDataSource source) => Obsolete10.AsDynamicForList();
        [PrivateApi] public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => Obsolete10.AsDynamicForList();

        #endregion
    }
}