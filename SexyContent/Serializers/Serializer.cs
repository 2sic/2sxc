using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ToSic.Eav;
using ToSic.SexyContent.EAVExtensions;

namespace ToSic.SexyContent.Serializers
{
	public class Serializer: Eav.Serializers.Serializer
	{
		private SexyContent Sxc { get; set; }

		public Serializer(SexyContent sexy)
		{
			Sxc = sexy;
		}


        #region Prepare statements expecting dynamic objects - extending the EAV Prepare variations

        /// <summary>
        /// Return an object that represents an IDataStream, but is serializable
        /// </summary>
        public IEnumerable<Dictionary<string, object>> Prepare(IEnumerable<dynamic> dynamicList)
        {
            var language = Thread.CurrentThread.CurrentCulture.Name;
            return dynamicList.Select(c => GetDictionaryFromEntity(c.Entity, language) as Dictionary<string, object>).ToList();
        }


        /// <summary>
        /// Return an object that represents an IDataStream, but is serializable
        /// </summary>
        public Dictionary<string, object> Prepare(DynamicEntity dynamicEntity)
        {
            return Prepare(dynamicEntity.Entity);
        }
        #endregion



        public override Dictionary<string, object> GetDictionaryFromEntity(IEntity entity, string language)
		{
            // Do groundwork
            var dictionary = base.GetDictionaryFromEntity(entity, language);

            // Add full presentation object if it has one...because there we need more than just id/title
			if (entity is EntityInContentGroup && !dictionary.ContainsKey("Presentation"))
			{
				var entityInGroup = (EntityInContentGroup)entity;
				if (entityInGroup.Presentation != null)
					dictionary.Add("Presentation", GetDictionaryFromEntity(entityInGroup.Presentation, language));
			}

            // Check w/2rm - this should only happen in edit mode...
		    if (true) // todo: edit-mode-only...?
		    {
                dictionary.Add("Modified", entity.Modified);
                
                if (entity is IHasEditingData)
		            dictionary.Add("_2sxcEditInformation", new {sortOrder = ((IHasEditingData) entity).SortOrder});
		        else
		            dictionary.Add("_2sxcEditInformation",
		                new {entityId = entity.EntityId, title = entity.Title != null ? entity.Title[language] : "(no title)"});
		    }
		    return dictionary;
		}
	}
}