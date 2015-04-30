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
		private SexyContent Sexy { get; set; }

		public Serializer(SexyContent sexy)
		{
			Sexy = sexy;
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
			return new Dictionary<string, object>() { { stream.Name, stream.List.Select(c => GetDictionaryFromEntity(c.Value, language)) } };
		}

		private Dictionary<string, object> GetDictionaryFromEntity(IEntity entity, string language)
		{
			var dynamicEntity = new DynamicEntity(entity, new[] { language }, Sexy);

			// Convert DynamicEntity to dictionary
			var dictionary = ((from d in entity.Attributes select d.Value).ToDictionary(k => k.Name, v =>
			{
				bool propertyNotFound;
				var value = dynamicEntity.GetEntityValue(v.Name, out propertyNotFound);
				if (v.Type == "Entity" && value is List<DynamicEntity>)
					return ((List<DynamicEntity>)value).Select(p => new { p.EntityId, p.EntityTitle });
				return value;
			}));

			dictionary.Add("EntityId", entity.EntityId);
			dictionary.Add("Modified", entity.Modified);

			if (entity is EntityInContentGroup && !dictionary.ContainsKey("Presentation"))
			{
				var entityInGroup = (EntityInContentGroup)entity;
				if (entityInGroup.Presentation != null)
					dictionary.Add("Presentation", GetDictionaryFromEntity(entityInGroup.Presentation, language));
			}

			if (entity is IHasEditingData)
				dictionary.Add("_2sxcEditInformation", new { sortOrder = ((IHasEditingData)entity).SortOrder });
			else
				dictionary.Add("_2sxcEditInformation", new { entityId = entity.EntityId, title = entity.Title != null ? entity.Title[language] : "(no title)" });

			return dictionary;
		}
	}
}