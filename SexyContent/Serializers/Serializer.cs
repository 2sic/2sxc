using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.SexyContent.EAVExtensions;

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
	    public Dictionary<string, object> Prepare(DynamicEntity dynamicEntity)
	        => GetDictionaryFromEntity(dynamicEntity.Entity);
        
        #endregion



        public override Dictionary<string, object> GetDictionaryFromEntity(IEntity entity)
		{
            // Do groundwork
            var dictionary = base.GetDictionaryFromEntity(entity);

            AddPresentation(entity, dictionary);
            AddEditInfo(entity, dictionary);

            return dictionary;
		}

	    internal void AddPresentation(IEntity entity, Dictionary<string, object> dictionary)
	    {
            // Add full presentation object if it has one...because there we need more than just id/title
	        if (entity is EntityInContentGroup && !dictionary.ContainsKey(Constants.PresentationKey))
	        {
	            var entityInGroup = (EntityInContentGroup) entity;
	            if (entityInGroup.Presentation != null)
	                dictionary.Add(Constants.PresentationKey, GetDictionaryFromEntity(entityInGroup.Presentation));//, language));
	        }
	    }

	    internal void AddEditInfo(IEntity entity, Dictionary<string, object> dictionary)
	    {
            // Add additional information in case we're in edit mode
	        if (DotNetNuke.Common.Globals.IsEditMode() || (Sxc?.Environment?.Permissions?.UserMayEditContent ?? false))
	        {
	            dictionary.Add(Constants.JsonModifiedNodeName, entity.Modified);

	            if (entity is IHasEditingData)
	                dictionary.Add(Constants.JsonEntityEditNodeName, new {sortOrder = ((IHasEditingData) entity).SortOrder});
	            else
	                dictionary.Add(Constants.JsonEntityEditNodeName,
	                    new {entityId = entity.EntityId, title = entity.Title != null ? entity.Title[Languages[0]] : "(no title)"});
	        }
	    }
	}
}