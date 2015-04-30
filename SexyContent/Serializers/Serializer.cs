using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.SexyContent.EAVExtensions;

namespace ToSic.SexyContent.Serializers
{
	public class Serializer
	{
		private SexyContent Sxc { get; set; }

		public Serializer(SexyContent sexy)
		{
			Sxc = sexy;
		}

		/// <summary>
		/// Returns an object that represents an IDataSource, but is serializable. If streamsToPublish is null, it will return all streams.
		/// </summary>
		public object Prepare(IDataSource source, IEnumerable<string> streamsToPublish = null)
		{
			var language = Thread.CurrentThread.CurrentCulture.Name;
			
			if (streamsToPublish == null)
				streamsToPublish = source.Out.Select(p => p.Key);

			var y = streamsToPublish.Where(k => source.Out.ContainsKey(k))
				.ToDictionary(k => k, s => source.Out[s].List.Select(c => GetDictionaryFromEntity(c.Value, language))
			);

			return y;
		}

		/// <summary>
		/// Returns an object that represents an IDataSource, but is serializable. If streamsToPublish is null, it will return all streams.
		/// </summary>
		public object Prepare(IDataSource source, string streamsToPublish)
		{
			return Prepare(source, streamsToPublish.Split(','));
		}

		/// <summary>
		/// Return an object that represents an IDataStream, but is serializable
		/// </summary>
		public object Prepare(IDataStream stream)
		{
			var language = Thread.CurrentThread.CurrentCulture.Name;
			return stream.List.Select(c => GetDictionaryFromEntity(c.Value, language));
		}

        /// <summary>
        /// Return an object that represents an IDataStream, but is serializable
        /// </summary>
        public object Prepare(IEntity entity)
        {
            var language = Thread.CurrentThread.CurrentCulture.Name;
            return GetDictionaryFromEntity(entity, language);
        }

        /// <summary>
        /// Return an object that represents an IDataStream, but is serializable
        /// </summary>
        public object Prepare(DynamicEntity dynamicEntity)
        {
            var language = Thread.CurrentThread.CurrentCulture.Name;
            return GetDictionaryFromEntity(dynamicEntity.Entity, language);
        }



		private Dictionary<string, object> GetDictionaryFromEntity(IEntity entity, string language)
		{
			var dynamicEntity = new DynamicEntity(entity, new[] { language }, Sxc);

			// Convert DynamicEntity to dictionary
			var dictionary = (from d in entity.Attributes select d.Value).ToDictionary(k => k.Name, v =>
			{
				bool propertyNotFound;
				var value = dynamicEntity.GetEntityValue(v.Name, out propertyNotFound);
				if (v.Type == "Entity" && value is List<DynamicEntity>)
					return ((List<DynamicEntity>)value).Select(p => new { Id = p.EntityId, Title = p.EntityTitle });
				return value;
			}, StringComparer.OrdinalIgnoreCase);

            // Add full presentation object if it has one...because there we need more than just id/title
			if (entity is EntityInContentGroup && !dictionary.ContainsKey("Presentation"))
			{
				var entityInGroup = (EntityInContentGroup)entity;
				if (entityInGroup.Presentation != null)
					dictionary.Add("Presentation", GetDictionaryFromEntity(entityInGroup.Presentation, language));
			}

            // If ID is not used by the entity itself as an internal value, give the object a Id property as well since it's nicer to use in JS
            // Note that for editing purposes or similar, there is always the extended info-object, so this is purely for "normal" working with the data
            dictionary.Add((dictionary.ContainsKey("Id") ? "EntityId": "Id"), entity.EntityId);
            if(!dictionary.ContainsKey("Title"))
                dictionary.Add("Title", entity.Title);

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