using ToSic.Eav;

namespace ToSic.SexyContent
{
    public class Template
    {
	    private readonly IEntity _templateEntity;
	    public Template(IEntity templateEntity)
	    {
		    _templateEntity = templateEntity;
	    }

		public int TemplateId { get { return _templateEntity.EntityId; } }

	    public string Name { get { return (string)_templateEntity.GetBestValue("Name"); } }
	    public string Path { get { return (string) _templateEntity.GetBestValue("Path"); } }

		public string ContentTypeStaticName { get { return (string)_templateEntity.GetBestValue("ContentTypeStaticName"); } }
		public IEntity ContentDemoEntity { get { return (IEntity)_templateEntity.GetBestValue("ContentDemoEntity"); } }
		public string PresentationTypeStaticName { get { return (string)_templateEntity.GetBestValue("Path"); } }
		public IEntity PresentationDemoEntity { get { return (IEntity)_templateEntity.GetBestValue("PresentationDemoEntity"); } }
		public string ListContentTypeStaticName { get { return (string)_templateEntity.GetBestValue("ListContentTypeStaticName"); } }
		public IEntity ListContentDemoEntity { get { return (IEntity)_templateEntity.GetBestValue("ListContentDemoEntity"); } }
		public string ListPresentationTypeStaticName { get { return (string)_templateEntity.GetBestValue("ListPresentationTypeStaticName"); } }
		public IEntity ListPresentationDemoEntity { get { return (IEntity)_templateEntity.GetBestValue("ListPresentationDemoEntity"); } }
		public string Type { get { return (string)_templateEntity.GetBestValue("Type"); } }
		public bool IsHidden { get { return (bool)_templateEntity.GetBestValue("IsHidden"); } }
		public string Location { get { return (string)_templateEntity.GetBestValue("Location"); } }
		public bool UseForList { get { return (bool)_templateEntity.GetBestValue("UseForList"); } }
		public bool PublishData { get { return (bool)_templateEntity.GetBestValue("PublishData"); } }
		public string StreamsToPublish { get { return (string)_templateEntity.GetBestValue("StreamsToPublish"); } }
		public IEntity Pipeline { get { return (IEntity)_templateEntity.GetBestValue("Pipeline"); } }
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