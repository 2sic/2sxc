using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.WebApi.App
{
    internal class AppContentEntityBuilder: HasLog
    {
        public AppContentEntityBuilder(ILog parentLog) : base("Api.Bldr", parentLog)
        {
        }


        /// <summary>
        /// Construct an import-friendly, type-controlled value-dictionary to create or update an entity
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> CreateEntityDictionary(string contentType, Dictionary<string, object> newContentItem, AppState appState/* int appId*/)
        {
            Log.Add($"create ent dic a#{appState.AppId}, type:{contentType}");
            // Retrieve content-type definition and check all the fields that this content-type has
            //var appState = Eav.Apps.State.Get(appId);
            var listOfTypes = appState.GetContentType(contentType);
            var attribs = listOfTypes.Attributes;


            var cleanedNewItem = new Dictionary<string, object>();
            foreach (var attrDef in attribs)
            {
                var attrName = attrDef.Name;
                if (!newContentItem.ContainsKey(attrName)) continue;
                var foundValue = newContentItem[attrName];
                switch (attrDef.Type.ToLowerInvariant())
                {
                    case "string":
                    case "hyperlink":
                        if (foundValue is string)
                            cleanedNewItem.Add(attrName, foundValue.ToString());
                        else
                            throw ValueMappingError(attrDef, foundValue);
                        break;
                    case "boolean":
                        if (bool.TryParse(foundValue.ToString(), out var bolValue))
                            cleanedNewItem.Add(attrName, bolValue);
                        else
                            throw ValueMappingError(attrDef, foundValue);
                        break;
                    case "datetime":
                        if (DateTime.TryParse(foundValue.ToString(), out var dtm))
                            cleanedNewItem.Add(attrName, dtm);
                        else
                            throw ValueMappingError(attrDef, foundValue);
                        break;
                    case "number":
                        if (decimal.TryParse(foundValue.ToString(), out var dec))
                            cleanedNewItem.Add(attrName, dec);
                        else
                            throw ValueMappingError(attrDef, foundValue);
                        break;
                    case "entity":
                        var relationships = new List<int>();

                        if (foundValue is IEnumerable foundEnum) // it's a list!
                            foreach (var item in foundEnum)
                                relationships.Add(CreateSingleRelationshipItem(item));
                        else // not a list
                            relationships.Add(CreateSingleRelationshipItem(foundValue));

                        cleanedNewItem.Add(attrName, relationships);

                        break;
                    default:
                        throw new Exception("Tried to create attribute '" + attrName + "' but the type is not known: '" +
                                            attrDef.Type + "'");
                }

                // todo: maybe one day get default-values and insert them if not supplied by JS
            }

            // Handle special attributes (for example of the system)
            if (newContentItem.ContainsKey(Attributes.EntityFieldIsPublished))
            {
                var foundValue = newContentItem[Attributes.EntityFieldIsPublished];
                if (bool.TryParse(foundValue.ToString(), out var bolValue))
                    cleanedNewItem.Add(Attributes.EntityFieldIsPublished, bolValue);
            }

            return cleanedNewItem;
        }

        /// <summary>
        /// In case of an error, show a nicer, consistent message
        /// </summary>
        /// <param name="attributeDefinition"></param>
        /// <param name="foundValue"></param>
        private static Exception ValueMappingError(IAttributeBase attributeDefinition, object foundValue)
            => new Exception("Tried to create " + attributeDefinition.Name
                             + " and couldn't convert to correct "
                             + attributeDefinition.Type + ": '" +
                             foundValue + "'");

        /// <summary>
        /// Takes input from JSON which could be in many formats like Category=ID or Category={id=#} 
        /// and then converts it to an item in the relationships-list
        /// </summary>
        /// <param name="foundValue"></param>
        private int CreateSingleRelationshipItem(object foundValue)
        {
            Log.Add("create relationship");
            try
            {
                // the object foundNumber is either just an Id, or an Id/Title combination
                // Try to see if it's already a number, else check if it's a JSON property
                if (!int.TryParse(foundValue.ToString(), out var foundNumber))
                {
                    if (foundValue is JProperty jp)
                        foundNumber = (int)jp.Value;
                    else
                    {
                        var jo = foundValue as JObject;
                        // ReSharper disable once PossibleNullReferenceException
                        if (jo.TryGetValue("Id", out var foundId))
                            foundNumber = (int)foundId;
                        else if (jo.TryGetValue("id", out foundId))
                            foundNumber = (int)foundId;
                    }
                }
                Log.Add($"relationship found:{foundNumber}");
                return foundNumber;
            }
            catch
            {
                throw new Exception("Tried to find Id of a relationship - but only found " + foundValue);
            }
        }
    }
}
