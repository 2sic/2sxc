using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using System.Web.WebPages;
using DotNetNuke.Entities.Modules;
using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.Eav.ValueProvider;
using ToSic.SexyContent.Adam;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Environment.Interfaces;
using ToSic.SexyContent.Razor.Helpers;
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
            if (!(parentPage is SexyContentWebPage)) return;    // 2016-02-22 believe this is necessary with dnn 8 because this razor uses a more complex inheritance with Type<T>
            //if (parentPage.GetType().BaseType != typeof(SexyContentWebPage)) return;
            
            Html = ((SexyContentWebPage) parentPage).Html;
            Url = ((SexyContentWebPage) parentPage).Url;
            Sexy = ((SexyContentWebPage) parentPage).Sexy;
            AppAndDataHelpers = ((SexyContentWebPage)parentPage).AppAndDataHelpers;
        }

        #endregion


        #region AppAndDataHelpers implementation

        public DnnHelper Dnn {
            get { return AppAndDataHelpers.Dnn; }
        }
		public SxcHelper Sxc
		{
			get { return AppAndDataHelpers.Sxc; }
		}
        public new App App {
            get { return AppAndDataHelpers.App; }
        }
        public ViewDataSource Data {
            get { return AppAndDataHelpers.Data; }
        }

        public IPermissions Permissions => Sexy.Environment.Permissions;

        #region AsDynamic in many variations
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
        #endregion


        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
        {
            return AppAndDataHelpers.CreateSource(typeName, inSource, configurationProvider);
        }

        public T CreateSource<T>(IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
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

            if(!File.Exists(HostingEnvironment.MapPath(path)))
                throw new FileNotFoundException("The shared file does not exist.", path);

            var webPage = (SexyContentWebPage)CreateInstanceFromVirtualPath(path);
            webPage.ConfigurePage(this);
            return webPage;
        }

        //public override HelperResult RenderPage(string path, params object[] data)
        //{
        //    if (String.IsNullOrEmpty(path))
        //    {
        //        throw ExceptionHelper.CreateArgumentNullOrEmptyException("path");
        //    }

        //    return new HelperResult(writer => {
        //        path = NormalizePath(path);
        //        Util.EnsureValidPageType(this, path);

        //        WebPageBase subPage = CreatePageFromVirtualPath(path);
        //        var pageContext = CreatePageContextFromParameters(isLayoutPage, data);

        //        subPage.ConfigurePage(this);
        //        subPage.ExecutePageHierarchy(pageContext, writer);
        //    });
        //}


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


        #region Adam 

        /// <summary>
        /// Provides an Adam instance for this item and field
        /// </summary>
        /// <param name="entity">The entity, often Content or similar</param>
        /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
        /// <returns>An Adam object for navigating the assets</returns>
        public AdamNavigator AsAdam(DynamicEntity entity, string fieldName)
        {
            return AppAndDataHelpers.AsAdam(AsEntity(entity), fieldName);
        }

        /// <summary>
        /// Provides an Adam instance for this item and field
        /// </summary>
        /// <param name="entity">The entity, often Content or similar</param>
        /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
        /// <returns>An Adam object for navigating the assets</returns>
        public AdamNavigator AsAdam(IEntity entity, string fieldName)
        {
            return AppAndDataHelpers.AsAdam(entity, fieldName);
        }
        #endregion

    }


}