using System;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource.Query;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Apps.Assets;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Blocks
{
    [PrivateApi("Internal implementation - don't publish")]
    public partial class View: EntityBasedWithLog, IView
    {

        #region Constructors

        public View(IEntity templateEntity, string[] languageCodes, ILog parentLog, LazySvc<QueryDefinitionBuilder> qDefBuilder) : base(templateEntity, languageCodes, parentLog, "Sxc.View")
        {
            _qDefBuilder = qDefBuilder;
        }
        private readonly LazySvc<QueryDefinitionBuilder> _qDefBuilder;

        #endregion

        private IEntity GetBestRelationship(string key) => Entity.Children(key).FirstOrDefault();


        public string Name => Get(FieldName, "unknown name");

        public string Identifier => Get(FieldIdentifier, "");
        
        public string Icon => Get(FieldIcon, "");

        public string Path => Get(FieldPath, "");

        public string ContentType => Get(FieldContentType, "");

        public IEntity ContentItem => GetBestRelationship(FieldContentDemo);

        public string PresentationType => Get(FieldPresentationType, "");

        public IEntity PresentationItem => GetBestRelationship(FieldPresentationItem);

        public string HeaderType => Get(FieldHeaderType, "");

        public IEntity HeaderItem => GetBestRelationship(FieldHeaderItem);

        public string HeaderPresentationType => Get(FieldHeaderPresentationType, "");

        public IEntity HeaderPresentationItem => GetBestRelationship(FieldHeaderPresentationItem);

        public string Type => Get(FieldType, "");

        [PrivateApi]
        internal string GetTypeStaticName(string groupPart)
        {
            switch (groupPart.ToLowerInvariant())
            {
                case ViewParts.ContentLower: return ContentType;
                case ViewParts.PresentationLower: return PresentationType;
                case ViewParts.ListContentLower: return HeaderType;
                case ViewParts.ListPresentationLower: return HeaderPresentationType;
                default: throw new NotSupportedException("Unknown group part: " + groupPart);
            }
        }

        public bool IsHidden => Get(FieldIsHidden, false);

        public bool IsShared => _isShared ?? (_isShared = AppAssets.IsShared(Get(FieldLocation, AppAssets.AppInSite))).Value;
        private bool? _isShared;

        public bool UseForList => Get(FieldUseList, false);
        public bool PublishData => Get(FieldPublishEnable, false);
        public string StreamsToPublish => Get(FieldPublishStreams, "");

        public IEntity QueryRaw => QueryInfo.Entity;

        public QueryDefinition Query => QueryInfo.Definition;

        private (IEntity Entity, QueryDefinition Definition) QueryInfo => _queryInfo.Get(() =>
        {
            var queryRaw = GetBestRelationship(FieldPipeline);
            var query = queryRaw != null
                ? _qDefBuilder.Value.Create(queryRaw, Entity.AppId)
                : null;
            return (queryRaw, query);
        });

        private readonly GetOnce<(IEntity Entity, QueryDefinition Definition)> _queryInfo =
            new GetOnce<(IEntity Entity, QueryDefinition Definition)>();

        public string UrlIdentifier => Entity.Value<string>(FieldNameInUrl);

        /// <summary>
        /// Returns true if the current template uses Razor
        /// </summary>
        public bool IsRazor => Type == TypeRazorValue;

        public string Edition { get; set; }

        public string EditionPath { get; set; }

        public IEntity Resources => GetBestRelationship(FieldResources);

        public IEntity Settings => GetBestRelationship(FieldSettings);

        /// <inheritdoc />
        public bool SearchIndexingDisabled => Get(FieldSearchDisabled, false);

        /// <inheritdoc />
        public string ViewController => Get(FieldViewController, "");

        /// <inheritdoc />
        public string SearchIndexingStreams => Get(FieldSearchStreams, "");
    }
}