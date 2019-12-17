using System;
using System.Collections;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Eav.Run;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Search;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Compatibility;
using ToSic.Sxc.Compatibility.RazorPermissions;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Search;
using ToSic.Sxc.Web;
using DynamicCode = ToSic.Sxc.Web.DynamicCode;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;
// ReSharper disable InheritdocInvalidUsage

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Razor
{
    /// <summary>
    /// The core page type for delivering a 2sxc page
    /// Provides context infos like the Dnn object, helpers like Edit and much more. 
    /// </summary>
    public abstract class SexyContentWebPage : 
        RazorComponentBase, 
        IRazorComponent, 
        IDynamicCodeBeforeV10,
#pragma warning disable 618
        IAppAndDataHelpers
#pragma warning restore 618
    {
        #region Helpers linked through AppAndData Helpers

        public ILinkHelper Link => DynCode.Link;


        /// <summary>
        /// Helper commands to enable in-page editing functionality
        /// Use it to check if edit is enabled, generate context-json infos and provide toolbar buttons
        /// </summary>
        public IInPageEditingSystem Edit => DynCode.Edit;

        public IDnnContext Dnn => DynCode.Dnn;

        /// <inheritdoc />
        [PrivateApi("try to remove")]
        public SxcHelper Sxc => DynCode.Sxc;

        /// <inheritdoc />
        public new IApp App => DynCode.App;

        /// <inheritdoc />
        public IBlockDataSource Data => DynCode.Data;

        public RazorPermissions Permissions => new RazorPermissions(DynCode.CmsBlock);

        #region AsDynamic in many variations

        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => DynCode.AsDynamic(entity);


        /// <inheritdoc />
        public dynamic AsDynamic(dynamic dynamicEntity) => DynCode.AsDynamic(dynamicEntity);

        // todo: only in "old" controller, not in new one
        /// <inheritdoc />
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) => DynCode.AsDynamic(entityKeyValuePair.Value);



        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IDataStream stream) => DynCode.AsDynamic(stream.List);

        /// <inheritdoc />
        public IEntity AsEntity(dynamic dynamicEntity) => DynCode.AsEntity(dynamicEntity);


        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => DynCode.AsDynamic(entities);

        #endregion

        #region AsList (experimental)

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(dynamic list)
            => throw new Exception("AsList is a new feature in 2sxc 10.20. To use it, change your template type to " + nameof(RazorComponent) + " see https://r.2sxc.org/RazorComponent");

        #endregion


        #region Compatibility with Eav.Interfaces.IEntity - introduced in 10.10
        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(Eav.Interfaces.IEntity entity) => DynCode.AsDynamic(entity);


        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity> entityKeyValuePair) => DynCode.AsDynamic(entityKeyValuePair.Value);

        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public IEnumerable<dynamic> AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities) => DynCode.AsDynamic(entities);
        #endregion


        #region Data Source Stuff
        /// <inheritdoc cref="ToSic.Sxc.Dnn.Web.IDynamicCode" />
        [Obsolete]
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, ILookUpEngine lookUpEngine = null)
            => DynCode.CreateSource(typeName, inSource, lookUpEngine);

        /// <inheritdoc cref="ToSic.Sxc.Dnn.Web.IDynamicCode" />
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null)
            where T : IDataSource
            => DynCode.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc cref="ToSic.Sxc.Dnn.Web.IDynamicCode" />
        public T CreateSource<T>(IDataStream inStream) where T : IDataSource
            => DynCode.CreateSource<T>(inStream);

        #endregion

        #region Content, Header, etc. and List
        public dynamic Content => DynCode.Content;

        [Obsolete("use Content.Presentation instead")]
        public dynamic Presentation => DynCode.Content?.Presentation;

        /// <summary>
        /// We are blocking this property on purpose, so that people will want to migrate to the new RazorComponent
        /// </summary>
        public dynamic Header 
            => throw new Exception("The header property is a new feature in 2sxc 10.20. To use it, change your template type to " + nameof(RazorComponent) + " see https://r.2sxc.org/RazorComponent");
            //=> DynCodeHelper.Header;

        [Obsolete("Use Header instead")]
        public dynamic ListContent => DynCode.Header;

        [Obsolete("Use Header.Presentation instead")]
        public dynamic ListPresentation => DynCode.Header?.Presentation;

        [Obsolete("This is an old way used to loop things - shouldn't be used any more - will be removed in a future version")]
        public List<Element> List => DynCode.List;

        /// <inheritdoc/>
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson)
            => throw new Exception("The AsDynamic(string) is a new feature in 2sxc 10.20. To use it, change your template type to " + nameof(RazorComponent) + " see https://r.2sxc.org/RazorComponent");

        #endregion

        #endregion

        #region Customize Data & Search

        /// <inheritdoc />
        public virtual void CustomizeData() {}

        /// <inheritdoc />
        public virtual void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IContainer moduleInfo,
            DateTime beginDate) { }

        [PrivateApi("this is the old signature, should still be supported")]
        public virtual void CustomizeSearch(Dictionary<string, List<ISearchInfo>> searchInfos, ModuleInfo moduleInfo, DateTime beginDate) { }

        public Purpose Purpose { get; internal set; }

        [Obsolete("left for compatibility, use Purpose instead")]
        public InstancePurposes InstancePurpose { get; internal set; }

        #endregion


        #region Adam 

        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName) => DynCode.AsAdam(entity, fieldName);


        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName) => DynCode.AsAdam(entity, fieldName);

        #endregion

    }


}