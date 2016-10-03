using System;
using System.Linq;
using System.Threading;
using ToSic.Eav;
using EntityRelationship = ToSic.Eav.Data.EntityRelationship;

namespace ToSic.SexyContent
{
    public class Template
    {
	    private readonly IEntity _templateEntity;
	    public Template(IEntity templateEntity)
	    {
			if(templateEntity == null)
				throw new Exception("Template entity is null");

		    _templateEntity = templateEntity;
	    }

		public int TemplateId => _templateEntity.EntityId;

        public string Name => (string)_templateEntity.GetBestValue("Name", new [] { Thread.CurrentThread.CurrentUICulture.Name});
        public string Path => (string) _templateEntity.GetBestValue("Path");

        public string ContentTypeStaticName => (string)_templateEntity.GetBestValue("ContentTypeStaticName");
        public IEntity ContentDemoEntity => (((EntityRelationship)_templateEntity.Attributes["ContentDemoEntity"][0]).FirstOrDefault());
        public string PresentationTypeStaticName => (string)_templateEntity.GetBestValue("PresentationTypeStaticName");
        public IEntity PresentationDemoEntity => (((EntityRelationship)_templateEntity.Attributes["PresentationDemoEntity"][0]).FirstOrDefault());
        public string ListContentTypeStaticName => (string)_templateEntity.GetBestValue("ListContentTypeStaticName");
        public IEntity ListContentDemoEntity => (((EntityRelationship)_templateEntity.Attributes["ListContentDemoEntity"][0]).FirstOrDefault());
        public string ListPresentationTypeStaticName => (string)_templateEntity.GetBestValue("ListPresentationTypeStaticName");
        public IEntity ListPresentationDemoEntity => (((EntityRelationship)_templateEntity.Attributes["ListPresentationDemoEntity"][0]).FirstOrDefault());
        public string Type => (string)_templateEntity.GetBestValue("Type");
        public Guid Guid => (Guid)_templateEntity.GetBestValue("EntityGuid");

        public string GetTypeStaticName(string groupPart)
        {
            switch(groupPart.ToLower())
            {
                case "content":
                    return ContentTypeStaticName;
                case "presentation":
                    return PresentationTypeStaticName;
                case "listcontent":
                    return ListContentTypeStaticName;
                case "listpresentation":
                    return ListPresentationTypeStaticName;
                default:
                    throw new NotSupportedException("Unknown group part: " + groupPart);
            }
        }

	    public bool IsHidden => (bool)(_templateEntity.GetBestValue("IsHidden") ?? false);

        public string Location => (string)_templateEntity.GetBestValue("Location");
        public bool UseForList => (bool) _templateEntity.GetBestValue("UseForList");
        public bool PublishData => (bool)_templateEntity.GetBestValue("PublishData");
        public string StreamsToPublish => (string)_templateEntity.GetBestValue("StreamsToPublish");

        public IEntity Pipeline => (((EntityRelationship)_templateEntity.Attributes["Pipeline"][0]).FirstOrDefault());
        public string ViewNameInUrl => (string)_templateEntity.GetBestValue("ViewNameInUrl");

        /// <summary>
        /// Returns true if the current template uses Razor
        /// </summary>
        public bool IsRazor => (Type == "C# Razor" || Type == "VB Razor");
    }
}