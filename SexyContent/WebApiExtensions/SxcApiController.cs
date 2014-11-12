using DotNetNuke.Web.Api;
using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Razor.Helpers;

namespace ToSic.SexyContent.WebApiExtensions
{

    [SupportedModules("2sxc-app")]
    public abstract class SxcApiController : DnnApiController
    {
        private SexyContent _sexyContent;
        private SexyContent _sexyContentUncached;

        private FrontApi _frontApi;
        private FrontApi FrontApi {
            get
            {
                if (_frontApi == null)
                {
                    var moduleInfo = Request.FindModuleInfo();
                    var viewDataSource = Sexy.GetViewDataSource(Request.FindModuleId(), SexyContent.HasEditPermission(moduleInfo), DotNetNuke.Common.Globals.IsEditMode());
                    _frontApi = new FrontApi(Sexy, moduleInfo, (ViewDataSource)viewDataSource);
                }
                return _frontApi;
            }
        }

        // Sexy object should not be accessible for other assemblies - just internal use
        internal SexyContent Sexy
        {
            get
            {
                if (_sexyContent == null)
                    _sexyContent = HttpRequestMessageExtensions.GetSxcOfModuleContext(Request);
                return _sexyContent;
            }
        }
        internal SexyContent SexyUncached
        {
            get
            {
                if (_sexyContentUncached == null)
                    _sexyContentUncached = HttpRequestMessageExtensions.GetUncachedSxcOfModuleContext(Request);
                return _sexyContentUncached;
            }
        }


        #region Front API implementation

        protected internal DnnHelper Dnn
        {
            get { return FrontApi.Dnn; }
        }
        protected internal new App App
        {
            get { return FrontApi.App; }
        }
        protected internal ViewDataSource Data
        {
            get { return FrontApi.Data; }
        }

        /// <summary>
        /// Transform a IEntity to a DynamicEntity as dynamic object
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public dynamic AsDynamic(IEntity entity)
        {
            return FrontApi.AsDynamic(entity);
        }

        /// <summary>
        /// Makes sure a dynamicEntity could be wrapped in AsDynamic()
        /// </summary>
        /// <param name="dynamicEntity"></param>
        /// <returns></returns>
        public dynamic AsDynamic(dynamic dynamicEntity)
        {
            return FrontApi.AsDynamic(dynamicEntity);
        }

        /// <summary>
        /// Returns the value of a KeyValuePair as DynamicEntity
        /// </summary>
        /// <param name="entityKeyValuePair"></param>
        /// <returns></returns>
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair)
        {
            return FrontApi.AsDynamic(entityKeyValuePair.Value);
        }

        /// <summary>
        /// In case AsDynamic is used with Data["name"]
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> AsDynamic(IDataStream stream)
        {
            return FrontApi.AsDynamic(stream.List);
        }

        /// <summary>
        /// In case AsDynamic is used with Data["name"].List
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> AsDynamic(IDictionary<int, IEntity> list)
        {
            return FrontApi.AsDynamic(list);
        }

        /// <summary>
        /// Transform a DynamicEntity dynamic object back to a IEntity instance
        /// </summary>
        /// <param name="dynamicEntity"></param>
        /// <returns></returns>
        public IEntity AsEntity(dynamic dynamicEntity)
        {
            return FrontApi.AsEntity(dynamicEntity);
        }

        /// <summary>
        /// Returns a list of DynamicEntities
        /// </summary>
        /// <param name="entities">List of entities</param>
        /// <returns></returns>
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities)
        {
            return FrontApi.AsDynamic(entities);
        }

        protected IDataSource CreateSource(string typeName = "", IDataSource inSource = null, IConfigurationProvider configurationProvider = null)
        {
            return FrontApi.CreateSource(typeName, inSource, configurationProvider);
        }

        protected T CreateSource<T>(IDataSource inSource = null, IConfigurationProvider configurationProvider = null)
        {
            return FrontApi.CreateSource<T>(inSource, configurationProvider);
        }

        /// <summary>
        /// Create a source with initial stream to attach...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inStream"></param>
        /// <returns></returns>
        protected T CreateSource<T>(IDataStream inStream)
        {
            return FrontApi.CreateSource<T>(inStream);
        }

        #endregion

    }
}
