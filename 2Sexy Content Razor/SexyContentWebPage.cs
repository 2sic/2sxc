using System;
using System.IO;
using System.Web;
using System.Web.WebPages;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using ToSic.SexyContent.DataSources.Tokens;
using ToSic.SexyContent.Engines.TokenEngine;
using ToSic.SexyContent.Razor.Helpers;
using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent.Razor
{
    public abstract class SexyContentWebPage : WebPageBase
    {
        #region Helpers

        protected internal DnnHelper Dnn { get; internal set; }

        protected internal HtmlHelper Html { get; internal set; }

        protected internal UrlHelper Url { get; internal set; }

        // <2sic>
        protected internal dynamic Content { get; internal set; }
        protected internal dynamic Presentation { get; internal set; }
        protected internal dynamic ListContent { get; internal set; }
        protected internal dynamic ListPresentation { get; internal set; }
        protected internal new App App { get; internal set; }
        protected internal List<Element> List { get; internal set; }
        protected internal IDataTarget Data { get; internal set; }

        /// <summary>
        /// Transform a IEntity to a DynamicEntity as dynamic object
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public dynamic AsDynamic(IEntity entity)
        {
            return new DynamicEntity(entity, new[] { System.Threading.Thread.CurrentThread.CurrentCulture.Name });
        }

        /// <summary>
        /// Makes sure a dynamicEntity could be wrapped in AsDynamic()
        /// </summary>
        /// <param name="dynamicEntity"></param>
        /// <returns></returns>
        public dynamic AsDynamic(dynamic dynamicEntity)
        {
            return dynamicEntity;
        }

        /// <summary>
        /// Transform a DynamicEntity dynamic object back to a IEntity instance
        /// </summary>
        /// <param name="dynamicEntity"></param>
        /// <returns></returns>
        public IEntity AsEntity(dynamic dynamicEntity)
        {
            return ((DynamicEntity)dynamicEntity).Entity;
        }

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
            
            Dnn = ((SexyContentWebPage) parentPage).Dnn;
            Html = ((SexyContentWebPage) parentPage).Html;
            Url = ((SexyContentWebPage) parentPage).Url;

            Content = ((SexyContentWebPage) parentPage).Content;
            Presentation = ((SexyContentWebPage) parentPage).Presentation;
            ListContent = ((SexyContentWebPage) parentPage).ListContent;
            ListPresentation = ((SexyContentWebPage) parentPage).ListPresentation;
            List = ((SexyContentWebPage) parentPage).List;
            Data = ((SexyContentWebPage) parentPage).Data;
            App = ((SexyContentWebPage) parentPage).App;
        }

        #endregion


        private ConfigurationProvider _configurationProvider;
        private ConfigurationProvider ConfigurationProvider
        {
            get
            {
                if (_configurationProvider == null)
                {
                    _configurationProvider = new ConfigurationProvider();
                    _configurationProvider.Sources.Add("querystring", new QueryStringPropertyAccess());
                    _configurationProvider.Sources.Add("app", new AppPropertyAccess(App));
                    _configurationProvider.Sources.Add("appsettings", new DynamicEntityPropertyAccess(App.Settings));
                    _configurationProvider.Sources.Add("appresources", new DynamicEntityPropertyAccess(App.Resources));
                }
                return _configurationProvider;
            }
        }

        protected IDataSource CreateSource(string typeName = "", IDataSource inSource = null, IConfigurationProvider configurationProvider = null)
        {
            if (configurationProvider == null)
                configurationProvider = ConfigurationProvider;

            if (inSource != null)
                return DataSource.GetDataSource(typeName, inSource.ZoneId, inSource.AppId, inSource, configurationProvider);
            
            var initialSource = SexyContent.GetInitialDataSource(SexyContent.GetZoneID(PortalSettings.Current.PortalId).Value, App.AppId);
            return typeName != "" ? DataSource.GetDataSource(typeName, initialSource.ZoneId, initialSource.AppId, initialSource, configurationProvider) : initialSource;
        }

        protected T CreateSource<T>(IDataSource inSource = null, IConfigurationProvider configurationProvider = null)
        {
            if (configurationProvider == null)
                configurationProvider = ConfigurationProvider;

            if (inSource != null)
                return DataSource.GetDataSource<T>(inSource.ZoneId, inSource.AppId, inSource, configurationProvider);

            var initialSource = SexyContent.GetInitialDataSource(SexyContent.GetZoneID(PortalSettings.Current.PortalId).Value, App.AppId);
            return DataSource.GetDataSource<T>(initialSource.ZoneId, initialSource.AppId, initialSource, configurationProvider);
        }


        /// <summary>
        /// Creates instances of the shared pages with the given relative path
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public dynamic Shared(string relativePath)
        {
            var path = NormalizePath(relativePath);

            if(!File.Exists(Server.MapPath(path)))
                throw new FileNotFoundException("The shared file does not exist.", path);

            var webPage = (SexyContentWebPage)CreateInstanceFromVirtualPath(path);
            webPage.ConfigurePage(this);
            return webPage;
        }

        

    }

    // <2sic> Removed DotNetNukeWebPage<T>:DotNetNukeWebPage
    // </2sic>

    
}