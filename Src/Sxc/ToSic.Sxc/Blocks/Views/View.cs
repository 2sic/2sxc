using System;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps.Assets;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Blocks
{
    public partial class View: EntityBasedWithLog, IView
    {
        #region Constructors

        public View(IEntity templateEntity, string languageCode, ILog parentLog) : base(templateEntity, languageCode, parentLog, "Sxc.View")
        {
        }
        
        public View(IEntity templateEntity, string[] languageCodes, ILog parentLog) : base(templateEntity, languageCodes, parentLog, "Sxc.View")
        {
        }

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

        // 2021-05-28 doesn't seem used anywhere, so we'll inline - delete in ca. 1 month or so
        //private string Location => Get(FieldLocation, AppAssets.AppInSite);

        public bool IsShared
        {
            get
            {
                if (_isShared != null) return _isShared.Value;
                var loc = Get(FieldLocation, AppAssets.AppInSite);
                _isShared = loc == AppAssets.HostFileSystem || loc == AppAssets.AppInGlobal;
                return _isShared.Value;
            }
        }

        private bool? _isShared;

        public bool UseForList => Get(FieldUseList, false);
        public bool PublishData => Get(FieldPublishEnable, false);
        public string StreamsToPublish => Get(FieldPublishStreams, "");

        [PrivateApi]
        public IEntity QueryRaw
        {
            get
            {
                InitializeQueryStuff();
                return _queryRaw;
            }
        }
        private IEntity _queryRaw;

        [PrivateApi]
        public QueryDefinition Query
        {
            get
            {
                InitializeQueryStuff();
                return _query;
            }
        }
        private QueryDefinition _query;

        private void InitializeQueryStuff()
        {
            if (_queryInitialized) return;
            _queryInitialized = true;
            _queryRaw = GetBestRelationship(FieldPipeline);
            if (_queryRaw != null)
                _query = new QueryDefinition(_queryRaw, Entity.AppId, Log);
        }
        private bool _queryInitialized;

        public string UrlIdentifier => Entity.Value<string>(FieldNameInUrl);

        /// <summary>
        /// Returns true if the current template uses Razor
        /// </summary>
        [PrivateApi]
        public bool IsRazor => Type == TypeRazorValue;

        [PrivateApi]
        public string Edition { get; set; }
        
        [PrivateApi("WIP 12.02")]
        public IEntity Resources => GetBestRelationship(FieldResources);

        [PrivateApi("WIP 12.02")]
        public IEntity Settings => GetBestRelationship(FieldSettings);
    }
}