using System.Collections.Generic;
using ToSic.Eav.DataSources;
using ToSic.Eav.Interfaces;
using ToSic.Eav.ValueProvider;
using ToSic.SexyContent;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Interfaces;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Edit.InPageEditingSystem;
using App = ToSic.SexyContent.App;
using Constants = ToSic.Eav.Constants;

namespace ToSic.Sxc.Code
{
    public abstract class SharedWithContext : Interfaces.IAppAndDataHelpers
    {
        public App App => Sxc?.SxcInstance?.App;
        public ViewDataSource Data => Sxc?.SxcInstance?.Data;
        public SxcHelper Sxc { get; private set; }

        internal Interfaces.IAppAndDataHelpers Parent;

        internal virtual void InitShared(Interfaces.IAppAndDataHelpers parent)
        {
            Parent = parent;
            Sxc = parent.Sxc;
        }

        public dynamic AsDynamic(IEntity entity) => Parent?.AsDynamic(entity);

        public dynamic AsDynamic(dynamic dynamicEntity) => Parent?.AsDynamic(dynamicEntity);

        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) =>
            Parent?.AsDynamic(entityKeyValuePair);

        public IEnumerable<dynamic> AsDynamic(IDataStream stream) => Parent?.AsDynamic(stream);

        public IEntity AsEntity(dynamic dynamicEntity) => Parent?.AsEntity(dynamicEntity);

        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => Parent?.AsDynamic(entities);

        public T CreateSource<T>(IDataStream inStream) => Parent.CreateSource<T>(inStream);

        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null,
            IValueCollectionProvider configurationProvider = null)
            => Parent?.CreateSource(typeName, inSource, configurationProvider);

        public T CreateSource<T>(IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
            => Parent.CreateSource<T>(inSource, configurationProvider);

        public FolderOfField AsAdam(DynamicEntity entity, string fieldName)
            => Parent?.AsAdam(entity, fieldName);

        public FolderOfField AsAdam(IEntity entity, string fieldName)
            => Parent?.AsAdam(entity, fieldName);

        public ILinkHelper Link => Parent?.Link;
        public IInPageEditingSystem Edit => Parent?.Edit;

        #region SharedCode - must also map previous path to use here
        public string SharedCodeVirtualRoot { get; set; }

        public dynamic SharedCode(string path, 
            string dontRelyOnParameterOrder = Constants.RandomProtectionParameter,
            string name = null,
            string relativePath = null,
            bool throwOnError = true) =>
            Parent?.SharedCode(path, dontRelyOnParameterOrder, name, SharedCodeVirtualRoot, throwOnError);

        #endregion
    }
}
