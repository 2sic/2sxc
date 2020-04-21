using System;
using System.Linq;
using System.Threading;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Blocks
{
    public partial class View: EntityBasedWithLog, IView
    {

        public View(IEntity templateEntity, ILog parentLog) : base(templateEntity, parentLog, "Sxc.View")
        {
        }

        private IEntity GetBestRelationship(string key) => Entity.Children(key).FirstOrDefault();


        public string Name => 
            Entity.GetBestValue<string>(FieldName, new[] {Thread.CurrentThread.CurrentUICulture.Name});

        public string Path => Entity.GetBestValue<string>(FieldPath);

        public string ContentType  
            => Entity.GetBestValue<string>(FieldContentType);

        public IEntity ContentItem => GetBestRelationship(FieldContentDemo);

        public string PresentationType => Entity.GetBestValue<string>(FieldPresentationType);

        public IEntity PresentationItem => GetBestRelationship(FieldPresentationItem);

        public string HeaderType => Entity.GetBestValue<string>(FieldHeaderType);

        public IEntity HeaderItem => GetBestRelationship(FieldHeaderItem);

        public string HeaderPresentationType => Entity.GetBestValue<string>(FieldHeaderPresentationType);

        public IEntity HeaderPresentationItem => GetBestRelationship(FieldHeaderPresentationItem);

        public string Type => Entity.GetBestValue<string>(FieldType);

        [PrivateApi]
        internal string GetTypeStaticName(string groupPart)
        {
            switch (groupPart.ToLower())
            {
                case ViewParts.ContentLower:
                    return ContentType;
                case ViewParts.PresentationLower:
                    return PresentationType;
                case ViewParts.ListContentLower:
                    return HeaderType;
                case ViewParts.ListPresentationLower:
                    return HeaderPresentationType;
                default:
                    throw new NotSupportedException("Unknown group part: " + groupPart);
            }
        }

        public bool IsHidden => Entity.GetBestValue<bool>(FieldIsHidden);

        public string Location => Entity.GetBestValue<string>(FieldLocation);
        public bool UseForList => Entity.GetBestValue<bool>(FieldUseList);
        public bool PublishData => Entity.GetBestValue<bool>(FieldPublishEnable);
        public string StreamsToPublish => Entity.GetBestValue<string>(FieldPublishStreams);

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

        public string UrlIdentifier => Entity.GetBestValue<string>(FieldNameInUrl);

        /// <summary>
        /// Returns true if the current template uses Razor
        /// </summary>
        [PrivateApi]
        public bool IsRazor => Type == TypeRazorValue;
    }
}