using System;
using System.Collections.Generic;
using ToSic.Eav.DataSources;
using ToSic.Eav.Interfaces;
using ToSic.Eav.ValueProvider;
using ToSic.SexyContent;
using ToSic.SexyContent.DataSources;
using ToSic.Sxc.Interfaces;
using ToSic.Sxc.Adam;
using App = ToSic.SexyContent.App;
using Constants = ToSic.Eav.Constants;

namespace ToSic.Sxc.Code
{
    public abstract class WithContext : IDynamicCode
    {
        public App App => Parent?.App;

        /// <summary>
        /// Note that data is only available if we have an instance, as it's the
        /// instance data, not the app data
        /// </summary>
        public ViewDataSource Data => Sxc?.SxcInstance?.Data;
        public SxcHelper Sxc { get; private set; }

        internal IDynamicCode Parent;

        internal virtual void InitShared(IDynamicCode parent)
        {
            Parent = parent;
            Sxc = parent.Sxc;
        }

        #region Content and Header

        public dynamic Content => Parent?.Content;
        public dynamic Header => Parent?.Header;

        #endregion


        #region AsDynamic and AsEntity
        public dynamic AsDynamic(IEntity entity) => Parent?.AsDynamic(entity);

        public dynamic AsDynamic(dynamic dynamicEntity) => Parent?.AsDynamic(dynamicEntity);

        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) 
            => Parent?.AsDynamic(entityKeyValuePair);

        public IEnumerable<dynamic> AsDynamic(IDataStream stream) => Parent?.AsDynamic(stream);

        public IEntity AsEntity(dynamic dynamicEntity) => Parent?.AsEntity(dynamicEntity);

        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => Parent?.AsDynamic(entities);

        #endregion

        #region CreateSource
        public T CreateSource<T>(IDataStream inStream) where T : IDataSource
            => Parent.CreateSource<T>(inStream);

        [Obsolete("use CreateSource<T> instead")]
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null,
            IValueCollectionProvider configurationProvider = null)
            => Parent?.CreateSource(typeName, inSource, configurationProvider);

        public T CreateSource<T>(IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
            where T : IDataSource
            => Parent.CreateSource<T>(inSource, configurationProvider);

        #endregion

        #region AsAdam
        public FolderOfField AsAdam(IDynamicEntity entity, string fieldName)
            => Parent?.AsAdam(entity, fieldName);

        public FolderOfField AsAdam(IEntity entity, string fieldName)
            => Parent?.AsAdam(entity, fieldName);
        #endregion

        #region Link and Edit
        public ILinkHelper Link => Parent?.Link;
        public IInPageEditingSystem Edit => Parent?.Edit;
        #endregion

        #region SharedCode - must also map previous path to use here
        public string SharedCodeVirtualRoot { get; set; }

        public dynamic CreateInstance(string virtualPath, 
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter,
            string name = null,
            string relativePath = null,
            bool throwOnError = true) =>
            Parent?.CreateInstance(virtualPath, dontRelyOnParameterOrder, name, SharedCodeVirtualRoot, throwOnError);

        #endregion
    }
}
