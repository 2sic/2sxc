using System;
using System.Collections.Generic;
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
		///// <summary>
		///// Returns a JSON string for the elements
		///// </summary>
		//public string GetJsonFromStreams(IDataSource source, string[] streamsToPublish)
		//{
		//	var language = Thread.CurrentThread.CurrentCulture.Name;

		//	var y = streamsToPublish.Where(k => source.Out.ContainsKey(k)).ToDictionary(k => k, s => new
		//	{
		//		List = (from c in source.Out[s].List select GetDictionaryFromEntity(c.Value, language)).ToList()
		//	});

		//	return JsonConvert.SerializeObject(y);
		//}

		//internal Dictionary<string, object> GetDictionaryFromEntity(IEntity entity, string language)
		//{
		//	////var dynamicEntity = new DynamicEntity(entity, new[] { language }, this);
		//	//bool propertyNotFound;

		//	//// Convert DynamicEntity to dictionary
		//	//var dictionary = ((from d in entity.Attributes select d.Value).ToDictionary(k => k.Name, v =>
		//	//{
		//	//	var value = dynamicEntity.GetEntityValue(v.Name, out propertyNotFound);
		//	//	if (v.Type == "Entity" && value is List<DynamicEntity>)
		//	//		return ((List<DynamicEntity>)value).Select(p => new { p.EntityId, p.EntityTitle });
		//	//	return value;
		//	//}));

		//	//dictionary.Add("EntityId", entity.EntityId);
		//	//dictionary.Add("Modified", entity.Modified);

		//	//if (entity is EntityInContentGroup && !dictionary.ContainsKey("Presentation"))
		//	//{
		//	//	var entityInGroup = (EntityInContentGroup)entity;
		//	//	if (entityInGroup.Presentation != null)
		//	//		dictionary.Add("Presentation", GetDictionaryFromEntity(entityInGroup.Presentation, language));
		//	//}

		//	//if (entity is IHasEditingData)
		//	//	dictionary.Add("_2sxcEditInformation", new { sortOrder = ((IHasEditingData)entity).SortOrder });
		//	//else
		//	//	dictionary.Add("_2sxcEditInformation", new { entityId = entity.EntityId, title = entity.Title != null ? entity.Title[language] : "(no title)" });

		//	//return dictionary;
		//}
	}
}