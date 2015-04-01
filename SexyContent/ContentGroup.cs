using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.Eav;

namespace ToSic.SexyContent
{
	public class ContentGroup
	{

		private readonly IEntity _contentGroupEntity;
	    public ContentGroup(IEntity contentGroupEntity)
	    {
			if (contentGroupEntity == null)
				throw new Exception("ContentGroup entity is null");

		    _contentGroupEntity = contentGroupEntity;
	    }

		public Template Template
		{
			get
			{
				var templateEntity = ((Eav.Data.EntityRelationship) _contentGroupEntity.Attributes["Template"][0]).FirstOrDefault();
				if (templateEntity == null)
					return null;
				return new Template(templateEntity);
			}
		}

		public int ContentGroupId { get { return _contentGroupEntity.EntityId; } }

		public List<IEntity> Content { get { return ((Eav.Data.EntityRelationship)_contentGroupEntity.GetBestValue("Content")).ToList(); } }
		public List<IEntity> Presentation { get { return ((Eav.Data.EntityRelationship)_contentGroupEntity.GetBestValue("Presentation")).ToList(); } }
		public List<IEntity> ListContent { get { return ((Eav.Data.EntityRelationship)_contentGroupEntity.GetBestValue("ListContent")).ToList(); } }
		public List<IEntity> ListPresentation { get { return ((Eav.Data.EntityRelationship)_contentGroupEntity.GetBestValue("ListPresentation")).ToList(); } }

	}
}