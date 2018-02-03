using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using System.Web.WebPages;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.DataSources;
using ToSic.Eav.Interfaces;
using ToSic.Eav.ValueProvider;
using ToSic.SexyContent.Adam;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Edit.InPageEditingSystem;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Interfaces;
using ToSic.SexyContent.Razor.Helpers;
using ToSic.SexyContent.Search;
using Factory = ToSic.Eav.Factory;

namespace ToSic.SexyContent.Razor
{
    /// <summary>
    /// The core page type for delivering a 2sxc page
    /// Provides context infos like the Dnn object, helpers like Edit and much more. 
    /// </summary>
    public abstract class SexyContentWebPage : WebPageBase, IAppAndDataHelpers
    {
        #region Helpers

        protected internal HtmlHelper Html { get; internal set; }

        protected internal UrlHelper Url { get; internal set; }

        protected internal ILinkHelper Link => DnnAppAndDataHelpers.Link;

        // <2sic>
        protected internal SxcInstance Sexy { get; set; }
        protected internal DnnAppAndDataHelpers DnnAppAndDataHelpers { get; set; }
        // </2sic>

        #endregion

        #region BaseClass Overrides

        protected override void ConfigurePage(WebPageBase parentPage)
        {
            base.ConfigurePage(parentPage);

            // Child pages need to get their context from the Parent
            Context = parentPage.Context;

            // Return if parent page is not a SexyContentWebPage

            // 2016-05-02 if (!(parentPage is SexyContentWebPage)) return;    // 2016-02-22 believe this is necessary with dnn 8 because this razor uses a more complex inheritance with Type<T>
            //if (parentPage.GetType().BaseType != typeof(SexyContentWebPage)) return;

            var typedParent = parentPage as SexyContentWebPage;
            if (typedParent == null) return;

            Html = typedParent.Html;
            Url = typedParent.Url;
            Sexy = typedParent.Sexy;
            DnnAppAndDataHelpers = typedParent.DnnAppAndDataHelpers;
        }

        #endregion


        #region AppAndDataHelpers implementation

        /// <inheritdoc />
        public DnnHelper Dnn => DnnAppAndDataHelpers.Dnn;

        /// <inheritdoc />
        public SxcHelper Sxc => DnnAppAndDataHelpers.Sxc;

        /// <inheritdoc />
        public new App App => DnnAppAndDataHelpers.App;

        /// <inheritdoc />
        public ViewDataSource Data => DnnAppAndDataHelpers.Data;

        public IPermissions Permissions => new RazorPermissions(Sexy); // Sexy.Environment.Permissions;

        // temp - should be elsewhere, but quickly need it so Permissions-object still works after refactoring
        internal class RazorPermissions: DnnPermissions
        {
            private readonly SxcInstance _sxcInstance;
            public RazorPermissions(SxcInstance sxc) => _sxcInstance = sxc;
            public bool UserMayEdit => Factory.Resolve<IPermissions>().UserMayEditContent(_sxcInstance.InstanceInfo);

        }

        #region AsDynamic in many variations
        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => DnnAppAndDataHelpers.AsDynamic(entity);
        

        /// <inheritdoc />
        public dynamic AsDynamic(dynamic dynamicEntity) =>  DnnAppAndDataHelpers.AsDynamic(dynamicEntity);


        /// <inheritdoc />
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) =>  DnnAppAndDataHelpers.AsDynamic(entityKeyValuePair.Value);


        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IDataStream stream) => DnnAppAndDataHelpers.AsDynamic(stream.List);


        /// <inheritdoc />
        public IEntity AsEntity(dynamic dynamicEntity) => DnnAppAndDataHelpers.AsEntity(dynamicEntity);


        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => DnnAppAndDataHelpers.AsDynamic(entities);

        #endregion

        #region Data Source Stuff
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
        {
            return DnnAppAndDataHelpers.CreateSource(typeName, inSource, configurationProvider);
        }

        public T CreateSource<T>(IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
        {
            return DnnAppAndDataHelpers.CreateSource<T>(inSource, configurationProvider);
        }

		/// <summary>
		/// Create a source with initial stream to attach...
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="inStream"></param>
		/// <returns></returns>
		public T CreateSource<T>(IDataStream inStream) =>  DnnAppAndDataHelpers.CreateSource<T>(inStream);

        #endregion

        #region Content, Presentation, ListContent, ListPresentation and List
        public dynamic Content => DnnAppAndDataHelpers.Content;

        public dynamic Presentation => DnnAppAndDataHelpers.Content?.Presentation;

        public dynamic ListContent => DnnAppAndDataHelpers.ListContent;

        public dynamic ListPresentation => DnnAppAndDataHelpers.ListContent?.Presentation;

        [Obsolete("This is an old way used to loop things - shouldn't be used any more - will be removed in a future version")]
        public List<Element> List => DnnAppAndDataHelpers.List;
        #endregion

        #endregion


        /// <summary>
        /// Creates instances of the shared pages with the given relative path
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public dynamic CreateInstance(string relativePath)
        {
            var path = NormalizePath(relativePath);

            if(!System.IO.File.Exists(HostingEnvironment.MapPath(path)))
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


        #region Adam 

        /// <summary>
        /// Provides an Adam instance for this item and field
        /// </summary>
        /// <param name="entity">The entity, often Content or similar</param>
        /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
        /// <returns>An Adam object for navigating the assets</returns>
        public AdamNavigator AsAdam(DynamicEntity entity, string fieldName) =>  DnnAppAndDataHelpers.AsAdam(entity, fieldName);
        

        /// <summary>
        /// Provides an Adam instance for this item and field
        /// </summary>
        /// <param name="entity">The entity, often Content or similar</param>
        /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
        /// <returns>An Adam object for navigating the assets</returns>
        public AdamNavigator AsAdam(IEntity entity, string fieldName) =>  DnnAppAndDataHelpers.AsAdam(entity, fieldName);

        #endregion

        #region Edit

        /// <summary>
        /// Helper commands to enable in-page editing functionality
        /// Use it to check if edit is enabled, generate context-json infos and provide toolbar buttons
        /// </summary>
        public IInPageEditingSystem Edit => DnnAppAndDataHelpers.Edit;

        #endregion
    }


}