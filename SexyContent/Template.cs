using System;
using System.Linq;
using ToSic.Eav;

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

		public int TemplateId { get { return _templateEntity.EntityId; } }

	    public string Name { get { return (string)_templateEntity.GetBestValue("Name"); } }
	    public string Path { get { return (string) _templateEntity.GetBestValue("Path"); } }

		public string ContentTypeStaticName { get { return (string)_templateEntity.GetBestValue("ContentTypeStaticName"); } }
		public IEntity ContentDemoEntity { get { return (((Eav.Data.EntityRelationship)_templateEntity.Attributes["ContentDemoEntity"][0]).FirstOrDefault()); } }
		public string PresentationTypeStaticName { get { return (string)_templateEntity.GetBestValue("PresentationTypeStaticName"); } }
		public IEntity PresentationDemoEntity { get { return (((Eav.Data.EntityRelationship)_templateEntity.Attributes["PresentationDemoEntity"][0]).FirstOrDefault()); } }
		public string ListContentTypeStaticName { get { return (string)_templateEntity.GetBestValue("ListContentTypeStaticName"); } }
		public IEntity ListContentDemoEntity { get { return (((Eav.Data.EntityRelationship)_templateEntity.Attributes["ListContentDemoEntity"][0]).FirstOrDefault()); } }
		public string ListPresentationTypeStaticName { get { return (string)_templateEntity.GetBestValue("ListPresentationTypeStaticName"); } }
		public IEntity ListPresentationDemoEntity { get { return (((Eav.Data.EntityRelationship)_templateEntity.Attributes["ListPresentationDemoEntity"][0]).FirstOrDefault()); } }
		public string Type { get { return (string)_templateEntity.GetBestValue("Type"); } }

	    public bool IsHidden
	    {
		    get
		    {
				return (bool)(_templateEntity.GetBestValue("IsHidden") ?? false);
		    }
	    }

	    public string Location { get { return (string)_templateEntity.GetBestValue("Location"); } }
		public bool UseForList {
			get
			{
				return (bool) _templateEntity.GetBestValue("UseForList");
			}
		}
		public bool PublishData { get { return (bool)_templateEntity.GetBestValue("PublishData"); } }
		public string StreamsToPublish { get { return (string)_templateEntity.GetBestValue("StreamsToPublish"); } }

		public IEntity Pipeline { get { return (((Eav.Data.EntityRelationship)_templateEntity.Attributes["Pipeline"][0]).FirstOrDefault()); } }
		public string ViewNameInUrl { get { return (string)_templateEntity.GetBestValue("ViewNameInUrl"); } }

        /// <summary>
        /// Returns true if the current template uses Razor
        /// </summary>
        public bool IsRazor
        {
            get
            {
                return (Type == "C# Razor" || Type == "VB Razor");
            }
        }

    }
}