using System;
using System.Collections.Generic;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.SexyContent;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Web;
using Constants = ToSic.Eav.Constants;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Code
{
    public abstract class WithContext : IDynamicCode
    {
        public IApp App => Parent?.App;

        /// <summary>
        /// Note that data is only available if we have an instance, as it's the
        /// instance data, not the app data
        /// </summary>
        public IBlockDataSource Data => Sxc?.Cms?.Block.Data;
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

        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(Eav.Interfaces.IEntity entity) => Parent?.AsDynamic(entity);

        public dynamic AsDynamic(dynamic dynamicEntity) => Parent?.AsDynamic(dynamicEntity);

        //public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) 
        //    => Parent?.AsDynamic(entityKeyValuePair);

        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity> entityKeyValuePair) 
            => Parent?.AsDynamic(entityKeyValuePair);


        public IEnumerable<dynamic> AsDynamic(IDataStream stream) => Parent?.AsDynamic(stream);

        public IEntity AsEntity(dynamic dynamicEntity) => Parent?.AsEntity(dynamicEntity);

        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => Parent?.AsDynamic(entities);

        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public IEnumerable<dynamic> AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities) => Parent?.AsDynamic(entities);


        #endregion

        #region CreateSource
        public T CreateSource<T>(IDataStream inStream) where T : IDataSource
            => Parent.CreateSource<T>(inStream);

        [Obsolete("use CreateSource<T> instead")]
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null,
            ITokenListFiller configurationProvider = null)
            => Parent?.CreateSource(typeName, inSource, configurationProvider);

        public T CreateSource<T>(IDataSource inSource = null, ITokenListFiller configurationProvider = null)
            where T : IDataSource
            => Parent.CreateSource<T>(inSource, configurationProvider);

        #endregion

        #region AsAdam
        public IFolder AsAdam(IDynamicEntity entity, string fieldName)
            => Parent?.AsAdam(entity, fieldName);

        public IFolder AsAdam(IEntity entity, string fieldName)
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
