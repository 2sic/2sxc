using System;
using System.Linq;
using System.Threading;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Engines
{
    public partial class Template: EntityBasedType
    {

        public Template(IEntity templateEntity) : base(templateEntity)
        {
        }

        private IEntity GetBestRelationship(string key)
            => Entity.Children(key).FirstOrDefault();


        public int TemplateId => Entity.EntityId;

        public string Name => 
            Entity.GetBestValue<string>(TemplateName, new[] {Thread.CurrentThread.CurrentUICulture.Name});

        public string Path 
            => Entity.GetBestValue<string>(TemplatePath);

        public string ContentTypeStaticName  
            => Entity.GetBestValue<string>(TemplateContentType);

        public IEntity ContentDemoEntity => GetBestRelationship(TemplateContentDemo);

        public string PresentationTypeStaticName => Entity.GetBestValue<string>(TemplatePresentationType);

        public IEntity PresentationDemoEntity => GetBestRelationship(TemplatePresentationDemo);

        public string ListContentTypeStaticName => Entity.GetBestValue<string>(TemplateListContentType);

        public IEntity ListContentDemoEntity => GetBestRelationship(TemplateListContentDemo);

        public string ListPresentationTypeStaticName => Entity.GetBestValue<string>(TemplateListPresentationType);

        public IEntity ListPresentationDemoEntity => GetBestRelationship(TemplateListPresentationDemo);

        public string Type => Entity.GetBestValue<string>(TemplateType);

        public string GetTypeStaticName(string groupPart)
        {
            switch (groupPart.ToLower())
            {
                case AppConstants.ContentLower:
                    return ContentTypeStaticName;
                case AppConstants.PresentationLower:
                    return PresentationTypeStaticName;
                case "listcontent":
                    return ListContentTypeStaticName;
                case "listpresentation":
                    return ListPresentationTypeStaticName;
                default:
                    throw new NotSupportedException("Unknown group part: " + groupPart);
            }
        }

        public bool IsHidden => Entity.GetBestValue<bool>(TemplateIsHidden);

        public string Location => Entity.GetBestValue<string>(TemplateLocation);
        public bool UseForList => Entity.GetBestValue<bool>(TemplateUseList);
        public bool PublishData => Entity.GetBestValue<bool>(TemplatePublishEnable);
        public string StreamsToPublish => Entity.GetBestValue<string>(TemplatePublishStreams);


        public IEntity Query => ((EntityRelationship)Entity.Attributes["Pipeline"][0]).FirstOrDefault();
        public string ViewNameInUrl => Entity.GetBestValue<string>(TemplateViewName);

        /// <summary>
        /// Returns true if the current template uses Razor
        /// </summary>
        [PrivateApi]
        public bool IsRazor => Type == TemplateTypeRazor;
    }
}