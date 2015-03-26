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
		    _contentGroupEntity = contentGroupEntity;
	    }

		public Template Template { get { return new Template(((IEntity)_contentGroupEntity.GetBestValue("Template"))); } }

		public int ContentGroupId { get { return _contentGroupEntity.EntityId; } }

		public List<IEntity> Content { get { return (List<IEntity>) _contentGroupEntity.GetBestValue("Content"); } }
		public List<IEntity> Presentation { get { return (List<IEntity>) _contentGroupEntity.GetBestValue("Presentation"); } }
		public List<IEntity> ListContent { get { return (List<IEntity>) _contentGroupEntity.GetBestValue("ListContent"); } }
		public List<IEntity> ListPresentation { get { return (List<IEntity>) _contentGroupEntity.GetBestValue("ListPresentation"); } }

	}
}