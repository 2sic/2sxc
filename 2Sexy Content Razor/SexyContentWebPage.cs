using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Permissions;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.DataSources.Tokens;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Engines.TokenEngine;
using ToSic.SexyContent.Razor.Helpers;
using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.SexyContent.Search;

namespace ToSic.SexyContent.Razor
{
    public abstract class SexyContentWebPage : WebPageBase, IAppAndDataHelpers
    {
        #region Helpers

        protected internal HtmlHelper Html { get; internal set; }

        protected internal UrlHelper Url { get; internal set; }

        // <2sic>
        protected internal SexyContent Sexy { get; set; }
        protected internal AppAndDataHelpers AppAndDataHelpers { get; set; }
        // </2sic>

        #endregion

        #region BaseClass Overrides

        protected override void ConfigurePage(WebPageBase parentPage)
        {
            base.ConfigurePage(parentPage);

            // Child pages need to get their context from the Parent
            Context = parentPage.Context;

            // Return if parent page is not a SexyContentWebPage
            if (parentPage.GetType().BaseType != typeof(SexyContentWebPage)) return;
            
            Html = ((SexyContentWebPage) parentPage).Html;
            Url = ((SexyContentWebPage) parentPage).Url;
            AppAndDataHelpers = ((SexyContentWebPage)parentPage).AppAndDataHelpers;
        }

        #endregion


        #region AppAndDataHelpers implementation

        public DnnHelper Dnn {
            get { return AppAndDataHelpers.Dnn; }
        }
        public new App App {
            get { return AppAndDataHelpers.App; }
        }
        public ViewDataSource Data {
            get { return AppAndDataHelpers.Data; }
        }

        /// <summary>
        /// Transform a IEntity to a DynamicEntity as dynamic object
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public dynamic AsDynamic(IEntity entity)
        {
            return AppAndDataHelpers.AsDynamic(entity);
        }

        /// <summary>
        /// Makes sure a dynamicEntity could be wrapped in AsDynamic()
        /// </summary>
        /// <param name="dynamicEntity"></param>
        /// <returns></returns>
        public dynamic AsDynamic(dynamic dynamicEntity)
        {
            return AppAndDataHelpers.AsDynamic(dynamicEntity);
        }

        /// <summary>
        /// Returns the value of a KeyValuePair as DynamicEntity
        /// </summary>
        /// <param name="entityKeyValuePair"></param>
        /// <returns></returns>
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair)
        {
            return AppAndDataHelpers.AsDynamic(entityKeyValuePair.Value);
        }

        /// <summary>
        /// In case AsDynamic is used with Data["name"]
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> AsDynamic(IDataStream stream)
        {
            return AppAndDataHelpers.AsDynamic(stream.List);
        }

        /// <summary>
        /// In case AsDynamic is used with Data["name"].List
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> AsDynamic(IDictionary<int, IEntity> list)
        {
            return AppAndDataHelpers.AsDynamic(list);
        }

        /// <summary>
        /// Transform a DynamicEntity dynamic object back to a IEntity instance
        /// </summary>
        /// <param name="dynamicEntity"></param>
        /// <returns></returns>
        public IEntity AsEntity(dynamic dynamicEntity)
        {
            return AppAndDataHelpers.AsEntity(dynamicEntity);
        }

        /// <summary>
        /// Returns a list of DynamicEntities
        /// </summary>
        /// <param name="entities">List of entities</param>
        /// <returns></returns>
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities)
        {
            return AppAndDataHelpers.AsDynamic(entities);
        }

        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, IConfigurationProvider configurationProvider = null)
        {
            return AppAndDataHelpers.CreateSource(typeName, inSource, configurationProvider);
        }

        public T CreateSource<T>(IDataSource inSource = null, IConfigurationProvider configurationProvider = null)
        {
            return AppAndDataHelpers.CreateSource<T>(inSource, configurationProvider);
        }

		/// <summary>
		/// Create a source with initial stream to attach...
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="inStream"></param>
		/// <returns></returns>
		public T CreateSource<T>(IDataStream inStream)
		{
		    return AppAndDataHelpers.CreateSource<T>(inStream);
		}

		public dynamic Content {
			get { return AppAndDataHelpers.Content; }
		}

	    public dynamic Presentation {
		    get { return AppAndDataHelpers.Presentation; }
	    }

	    public dynamic ListContent {
		    get { return AppAndDataHelpers.ListContent; }
	    }

		public dynamic ListPresentation {
			get { return AppAndDataHelpers.ListPresentation; }
		}

		public List<Element> List {
			get { return AppAndDataHelpers.List; }
		}

        #endregion


        /// <summary>
        /// Creates instances of the shared pages with the given relative path
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public dynamic CreateInstance(string relativePath)
        {
            var path = NormalizePath(relativePath);

            if(!File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(path)))
                throw new FileNotFoundException("The shared file does not exist.", path);

            var webPage = (SexyContentWebPage)CreateInstanceFromVirtualPath(path);
            webPage.ConfigurePage(this);
            return webPage;
        }


        /// <summary>
        /// Override this to have your code change the (already initialized) Data object. 
        /// If you don't override this, nothing will be changed/customized. 
        /// </summary>
        public virtual void CustomizeData()
        {
        }

        public virtual void CustomizeSearch(Dictionary<string, List<ISearchInfo>> searchInfos, ModuleInfo moduleInfo, DateTime beginDate)
        {
        }

        public InstancePurposes InstancePurpose { get; set; }


    }

    // <2sic> Removed DotNetNukeWebPage<T>:DotNetNukeWebPage
    // </2sic>

    
}