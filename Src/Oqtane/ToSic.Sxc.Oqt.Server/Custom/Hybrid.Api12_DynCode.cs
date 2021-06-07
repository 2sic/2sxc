using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using ToSic.Eav;
using ToSic.Eav.Configuration;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Adam;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Api12
    {

        // ReSharper disable once InconsistentNaming
        public IDynamicCodeRoot _DynCodeRoot { get; set; }

        [PrivateApi] public int CompatibilityLevel => _DynCodeRoot.CompatibilityLevel;

        public TService GetService<TService>() => ServiceProvider.Build<TService>();

        /// <inheritdoc />
        public IApp App => _DynCodeRoot?.App;

        /// <inheritdoc />
        public IBlockDataSource Data => _DynCodeRoot?.Data;

        #region AsDynamic implementations
        /// <inheritdoc/>
        [NonAction]
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => _DynCodeRoot?.AsDynamic(json, fallback);

        /// <inheritdoc />
        [NonAction]
        public dynamic AsDynamic(IEntity entity) => _DynCodeRoot?.AsDynamic(entity);

        /// <inheritdoc />
        [NonAction]
        public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot?.AsDynamic(dynamicEntity);

        /// <inheritdoc />
        [NonAction]
        public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot?.AsEntity(dynamicEntity);

        #endregion

        #region AsList

        /// <inheritdoc />
        [NonAction]
        public IEnumerable<dynamic> AsList(object list) => _DynCodeRoot?.AsList(list);

        #endregion

        #region CreateSource implementations

        /// <inheritdoc />
        [NonAction]
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null)
            where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc />
        [NonAction]
        public T CreateSource<T>(IDataStream inStream) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inStream);

        #endregion

        #region Content, Presentation & List

        /// <inheritdoc />
        public new dynamic Content => _DynCodeRoot?.Content;

        /// <inheritdoc />
        public dynamic Header => _DynCodeRoot?.Header;


        #endregion

        #region Adam

        /// <inheritdoc />
        [NonAction]
        public IFolder AsAdam(IDynamicEntity entity, string fieldName) => _DynCodeRoot?.AsAdam(AsEntity(entity), fieldName);

        /// <inheritdoc />
        [NonAction]
        public IFolder AsAdam(IEntity entity, string fieldName) => _DynCodeRoot?.AsAdam(entity, fieldName);


        // Adam - Shared Code Across the APIs (prevent duplicate code)

        /// <summary>
        /// See docs of official interface <see cref="IDynamicWebApi"/>
        /// </summary>
        [NonAction]
        public ToSic.Sxc.Adam.IFile SaveInAdam(string dontRelyOnParameterOrder = Parameters.Protector,
            Stream stream = null,
            string fileName = null,
            string contentType = null,
            Guid? guid = null,
            string field = null,
            string subFolder = "")
        {
            Parameters.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "SaveInAdam",
                $"{nameof(stream)},{nameof(fileName)},{nameof(contentType)},{nameof(guid)},{nameof(field)},{nameof(subFolder)} (optional)");

            if (stream == null || fileName == null || contentType == null || guid == null || field == null)
                throw new Exception();

            var feats = new[] { FeatureIds.UseAdamInWebApi, FeatureIds.PublicUpload };
            if (!Features.EnabledOrException(feats, "can't save in ADAM", out var exp))
                throw exp;

            var appId = _DynCodeRoot?.Block?.AppId ?? _DynCodeRoot?.App?.AppId ?? throw new Exception("Error, SaveInAdam needs an App-Context to work, but the App is not known.");
            return ServiceProvider.Build<AdamTransUpload<int, int>>()
                .Init(appId, contentType, guid.Value, field, false, Log)
                .UploadOne(stream, fileName, subFolder, true);
        }

        #endregion

        #region Link & Edit - added to API in 2sxc 10.01

        /// <inheritdoc />
        public ILinkHelper Link => _DynCodeRoot?.Link;

        /// <inheritdoc />
        public IInPageEditingSystem Edit => _DynCodeRoot?.Edit;

        #endregion

        #region  CreateInstance implementation

        public string CreateInstancePath { get; set; }

        [NonAction]
        public dynamic CreateInstance(string virtualPath,
            string dontRelyOnParameterOrder = Parameters.Protector,
            string name = null,
            string relativePath = null,
            bool throwOnError = true) =>
            _DynCodeRoot.CreateInstance(virtualPath, dontRelyOnParameterOrder, name, CreateInstancePath, throwOnError);

        #endregion


        #region CmsContext

        public ICmsContext CmsContext => _DynCodeRoot?.CmsContext;

        [PrivateApi("WIP 12.02")]
        public dynamic Resources => _DynCodeRoot.Resources;

        [PrivateApi("WIP 12.02")]
        public dynamic Settings => _DynCodeRoot.Settings;


        #endregion
    }
}
