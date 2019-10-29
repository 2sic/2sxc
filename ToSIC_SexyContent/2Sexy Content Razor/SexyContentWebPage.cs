using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using System.Web.WebPages;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.Interfaces;
using ToSic.Eav.ValueProvider;
using ToSic.Sxc.Adam;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.Sxc.Interfaces;
using ToSic.SexyContent.Search;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Interfaces;
using ToSic.Sxc.Razor;
using ToSic.Sxc.Razor.Interfaces;
using File = System.IO.File;

namespace ToSic.SexyContent.Razor
{
    /// <summary>
    /// The core page type for delivering a 2sxc page
    /// Provides context infos like the Dnn object, helpers like Edit and much more. 
    /// </summary>
    public abstract class SexyContentWebPage : WebPageBase, Sxc.Interfaces.IAppAndDataHelpers, IHasDnnContext
    {
        #region Helpers

        protected internal IHtmlHelper Html { get; internal set; }

        public ILinkHelper Link => DnnAppAndDataHelpers.Link;

        // <2sic>
        [PrivateApi]
        protected internal SxcInstance Sexy { get; set; }
        [PrivateApi]
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
            if (!(parentPage is SexyContentWebPage typedParent)) return;

            Html = typedParent.Html;
            // Deprecated 2019-05-27 2dm - I'm very sure this isn't used anywhere or by anyone.
            // reactivate if it turns out to be used, otherwise delete ca. EOY 2019
            //Url = typedParent.Url;
            Sexy = typedParent.Sexy;
            DnnAppAndDataHelpers = typedParent.DnnAppAndDataHelpers;
        }

        #endregion


        #region AppAndDataHelpers implementation

        /// <summary>
        /// Helper commands to enable in-page editing functionality
        /// Use it to check if edit is enabled, generate context-json infos and provide toolbar buttons
        /// </summary>
        public IInPageEditingSystem Edit => DnnAppAndDataHelpers.Edit;

        public IDnnContext Dnn => DnnAppAndDataHelpers.Dnn;

        /// <inheritdoc />
        public SxcHelper Sxc => DnnAppAndDataHelpers.Sxc;

        /// <inheritdoc />
        public new App App => DnnAppAndDataHelpers.App;

        /// <inheritdoc />
        public ViewDataSource Data => DnnAppAndDataHelpers.Data;

        public RazorPermissions Permissions => new RazorPermissions(Sexy);

        #region AsDynamic in many variations
        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => DnnAppAndDataHelpers.AsDynamic(entity);


        /// <inheritdoc />
        public dynamic AsDynamic(dynamic dynamicEntity) => DnnAppAndDataHelpers.AsDynamic(dynamicEntity);


        /// <inheritdoc />
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) => DnnAppAndDataHelpers.AsDynamic(entityKeyValuePair.Value);


        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IDataStream stream) => DnnAppAndDataHelpers.AsDynamic(stream.List);


        /// <inheritdoc />
        public IEntity AsEntity(dynamic dynamicEntity) => DnnAppAndDataHelpers.AsEntity(dynamicEntity);


        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => DnnAppAndDataHelpers.AsDynamic(entities);

        #endregion

        #region Data Source Stuff
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
            => DnnAppAndDataHelpers.CreateSource(typeName, inSource, configurationProvider);

        public T CreateSource<T>(IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
            => DnnAppAndDataHelpers.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc />
        /// <summary>
        /// Create a source with initial stream to attach...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inStream"></param>
        /// <returns></returns>
		public T CreateSource<T>(IDataStream inStream)
            => DnnAppAndDataHelpers.CreateSource<T>(inStream);

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



        private dynamic CreateInstanceCshtml(string path)
        {
            var webPage = (SexyContentWebPage)CreateInstanceFromVirtualPath(path);
            webPage.ConfigurePage(this);
            return webPage;
        }

        private static void VerifyFileExists(string path)
        {
            if (!File.Exists(HostingEnvironment.MapPath(path)))
                throw new FileNotFoundException("The shared file does not exist.", path);
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
        public FolderOfField AsAdam(IDynamicEntity entity, string fieldName) => DnnAppAndDataHelpers.AsAdam(entity, fieldName);


        /// <summary>
        /// Provides an Adam instance for this item and field
        /// </summary>
        /// <param name="entity">The entity, often Content or similar</param>
        /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
        /// <returns>An Adam object for navigating the assets</returns>
        public FolderOfField AsAdam(IEntity entity, string fieldName) => DnnAppAndDataHelpers.AsAdam(entity, fieldName);

        #endregion

        #region Compile Helpers

        public string SharedCodeVirtualRoot { get; set; }

        /// <summary>
        /// Creates instances of the shared pages with the given relative path
        /// </summary>
        /// <returns></returns>
        public dynamic CreateInstance(string virtualPath,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string name = null,
            string relativePath = null,
            bool throwOnError = true)
        {
            var path = NormalizePath(virtualPath);
            VerifyFileExists(path);
            return path.EndsWith(CodeCompiler.CsFileExtension)
                ? DnnAppAndDataHelpers.CreateInstance(path, dontRelyOnParameterOrder, name, null, throwOnError)
                : CreateInstanceCshtml(path);
        }

        #endregion
    }


}