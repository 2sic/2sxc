using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Serializers;
using ToSic.SexyContent.EAVExtensions;
using ToSic.SexyContent.Interfaces;
using ToSic.Sxc.Interfaces;

namespace ToSic.SexyContent.Serializers
{
	public class Serializer: Eav.Serializers.Serializer
	{
		public SxcInstance Sxc { get; set; }

        /// <summary>
        /// Standard constructor, important for Unity when opening this class in dependency-injection mode
        /// </summary>
	    public Serializer()
        {
	        
	    }

	    /// <summary>
	    /// Common constructor, directly preparing it with 2sxc
	    /// </summary>
	    /// <param name="sxcInstance"></param>
	    /// <param name="languages"></param>
	    public Serializer(SxcInstance sxcInstance, string[] languages = null)
        {
            Sxc = sxcInstance;
            Languages = languages;
        }


        #region Prepare statements expecting dynamic objects - extending the EAV Prepare variations

	    /// <summary>
	    /// Return an object that represents an IDataStream, but is serializable
	    /// </summary>
	    public IEnumerable<Dictionary<string, object>> Prepare(IEnumerable<dynamic> dynamicList)
	        => dynamicList.Select(c => GetDictionaryFromEntity(c.Entity) as Dictionary<string, object>).ToList();



	    /// <summary>
	    /// Return an object that represents an IDataStream, but is serializable
	    /// </summary>
	    public Dictionary<string, object> Prepare(Sxc.Interfaces.IDynamicEntity dynamicEntity)
	        => GetDictionaryFromEntity(dynamicEntity.Entity);
        
        #endregion



        public override Dictionary<string, object> GetDictionaryFromEntity(Eav.Interfaces.IEntity entity)
		{
            // Do groundwork
            var dictionary = base.GetDictionaryFromEntity(entity);

            AddPresentation(entity, dictionary);
            AddEditInfo(entity, dictionary);

            return dictionary;
		}

        #region to enhance serializable IEntities with 2sxc specific infos

        #region special "old" serializer which provides data in the older format
        internal Dictionary<string, object> PrepareOldFormat(Eav.Interfaces.IEntity entity)
        {
            // var ser = new Serializer(SxcInstance, _dimensions);
            var dicNew = GetDictionaryFromEntity(entity);
            var dicToSerialize = ConvertNewSerRelToOldSerRel(dicNew);

            dicToSerialize.Add(Constants.JsonEntityIdNodeName, entity.EntityId);

            return dicToSerialize;
        }

        internal Dictionary<string, object> ConvertNewSerRelToOldSerRel(Dictionary<string, object> dicNew)
        {
            // find all items which are of type List<SerializableRelationship>
            // then convert to EntityId and EntityTitle to conform to "old" format
            var dicToSerialize = new Dictionary<string, object>();
            foreach (string key in dicNew.Keys)
            {
                var list = dicNew[key] as List<SerializableRelationship>;
                dicToSerialize.Add(key,
                    list?.Select(p => new SerializableRelationshipOld() { EntityId = p.Id, EntityTitle = p.Title }).ToList() ??
                    dicNew[key]);
            }
            return dicToSerialize;
        }

        // Helper to provide old interface with "EntityId" and "EntityTitle" instead of 
        // "Id" and "Title"
        public class SerializableRelationshipOld
        {
            public int? EntityId;
            public object EntityTitle;
        }

        #endregion

        internal void AddPresentation(Eav.Interfaces.IEntity entity, Dictionary<string, object> dictionary)
	    {
            // Add full presentation object if it has one...because there we need more than just id/title
	        if (entity is EntityInContentGroup && !dictionary.ContainsKey(AppConstants.Presentation))
	        {
	            var entityInGroup = (EntityInContentGroup) entity;
	            if (entityInGroup.Presentation != null)
	                dictionary.Add(AppConstants.Presentation, GetDictionaryFromEntity(entityInGroup.Presentation));//, language));
	        }
	    }

	    internal void AddEditInfo(Eav.Interfaces.IEntity entity, Dictionary<string, object> dictionary)
	    {
            // Add additional information in case we're in edit mode
	        var userMayEdit = Sxc?.UserMayEdit ?? false;// Factory.Resolve<IPermissions>().UserMayEditContent(Sxc?.InstanceInfo);

	        if (!userMayEdit) return;

	        dictionary.Add(Constants.JsonModifiedNodeName, entity.Modified);
	        var title = entity.GetBestTitle(Languages);
	        if (string.IsNullOrEmpty(title))
	            title = "(no title)";
	        dictionary.Add(Constants.JsonEntityEditNodeName, entity is IHasEditingData
	            ? (object) new {
	                sortOrder = ((IHasEditingData) entity).SortOrder,
	                isPublished = entity.IsPublished,
	            }
	            : new {
	                entityId = entity.EntityId,
	                title,
	                isPublished = entity.IsPublished,
	            });
	    }

        #endregion
    }



}