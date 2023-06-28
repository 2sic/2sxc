using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Lib.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Code.Help;
using ToSic.Sxc.Compatibility;

namespace ToSic.Sxc.WebApi
{
    public partial class ApiCoreShim: IDynamicCodeBeforeV10
    {
        // Obsolete stuff - not supported any more in after V10 - show helpful error messages

        #region Shared Code Block between RazorComponent_Obsolete and ApiController_Obsolete

        #region Obsolete CreateSource

        [PrivateApi]
        [Obsolete("throws error with fix-instructions. Use CreateSource<type> instead.")]
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, ILookUpEngine configurationProvider = null)
            => CodeHelpDbV12.CreateSourceString();

        #endregion

        #region Compatibility with Eav.Interfaces.IEntity - introduced in 10.10

        [PrivateApi]
        [Obsolete("throws error with fix-instructions. Cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(ToSic.Eav.Interfaces.IEntity entity)
            => CodeHelpDbV12.AsDynamicInterfacesIEntity();


        [PrivateApi]
        [Obsolete("throws error with fix-instructions. Cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(KeyValuePair<int, ToSic.Eav.Interfaces.IEntity> entityKeyValuePair)
            => CodeHelpDbV12.AsDynamicKvpInterfacesIEntity();

        [Obsolete("throws error with fix-instructions. Cast your entities to ToSic.Eav.Data.IEntity")]
        [PrivateApi]
        public IEnumerable<dynamic> AsDynamic(IEnumerable<ToSic.Eav.Interfaces.IEntity> entities)
            => CodeHelpDbV12.AsDynamicIEnumInterfacesIEntity();


        #endregion

        #region AsDynamic<int, IEntity>

        [PrivateApi]
        [Obsolete("throws error with fix-instructions. Use AsDynamic(IEnumerable<IEntity>...)")]
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) => CodeHelpDbV12.AsDynamicKvp();

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
        public dynamic Presentation => CodeHelpDbV12.Presentation();


        [PrivateApi]
        [Obsolete("Use Header instead")]
        public dynamic ListContent => CodeHelpDbV12.ListContent();

        [PrivateApi]
        [Obsolete("Use Header.Presentation instead")]
        public dynamic ListPresentation => CodeHelpDbV12.ListPresentation();

        [PrivateApi]
        [Obsolete("This is an old way used to loop things - removed in RazorComponent")]
        public IEnumerable<dynamic> List => CodeHelpDbV12.List();

        #endregion

        #endregion

    }
}
