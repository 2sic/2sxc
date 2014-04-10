using System.Web.WebPages;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
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

        public dynamic AsDynamic(IEntity entity)
        {
            return new DynamicEntity(entity, new[] { System.Threading.Thread.CurrentThread.CurrentCulture.Name });
        }

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


        protected IDataSource CreateSource(string typeName = "", IDataSource inSource = null, IConfigurationProvider configurationProvider = null)
        {
            if (configurationProvider == null)
                configurationProvider = SexyContent.GetConfigurationProvider(App);

            if (inSource != null)
                return DataSource.GetDataSource(typeName, inSource.ZoneId, inSource.AppId, inSource, configurationProvider);
            
            var initialSource = SexyContent.GetInitialDataSource(SexyContent.GetZoneID(PortalSettings.Current.PortalId).Value, App.AppId);
            return typeName != "" ? DataSource.GetDataSource(typeName, initialSource.ZoneId, initialSource.AppId, initialSource, configurationProvider) : initialSource;
        }

        protected T CreateSource<T>(IDataSource inSource = null, IConfigurationProvider configurationProvider = null)
        {
            if (configurationProvider == null)
                configurationProvider = SexyContent.GetConfigurationProvider(App);

            if (inSource != null)
                return DataSource.GetDataSource<T>(inSource.ZoneId, inSource.AppId, inSource, configurationProvider);

            var initialSource = SexyContent.GetInitialDataSource(SexyContent.GetZoneID(PortalSettings.Current.PortalId).Value, App.AppId);
            return DataSource.GetDataSource<T>(initialSource.ZoneId, initialSource.AppId, initialSource, configurationProvider);
        }
    }

    // <2sic> Removed DotNetNukeWebPage<T>:DotNetNukeWebPage
    // </2sic>
}