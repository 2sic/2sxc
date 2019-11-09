using System;
using System.Linq;
using System.Threading;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Views
{
    public partial class View: EntityBasedType, IView
    {

        public View(IEntity templateEntity) : base(templateEntity)
        {
        }

        private IEntity GetBestRelationship(string key)
            => Entity.Children(key).FirstOrDefault();


        public string Name => 
            Entity.GetBestValue<string>(FieldName, new[] {Thread.CurrentThread.CurrentUICulture.Name});

        public string Path 
            => Entity.GetBestValue<string>(FieldPath);

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
                case AppConstants.ContentLower:
                    return ContentType;
                case AppConstants.PresentationLower:
                    return PresentationType;
                case "listcontent":
                    return HeaderType;
                case "listpresentation":
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


        public IEntity Query => GetBestRelationship(FieldPipeline);

        public string UrlIdentifier => Entity.GetBestValue<string>(FieldNameInUrl);

        /// <summary>
        /// Returns true if the current template uses Razor
        /// </summary>
        [PrivateApi]
        public bool IsRazor => Type == TypeRazorValue;
    }
}