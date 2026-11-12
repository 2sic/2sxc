using System.Collections;
using System.Text.Json;
using System.Text.Json.Nodes;
using ToSic.Eav.Apps.Sys.Api01;
using ToSic.Sys.Utils;

namespace ToSic.Eav.WebApi.Sys.App;

internal class AppContentEntityBuilder(ILog parentLog) : HelperBase(parentLog, "Api.Bldr")
{
    /// <summary>
    /// Construct an import-friendly, type-controlled value-dictionary to create or update an entity
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object> CreateEntityDictionary<T>(string contentType, Dictionary<string, object?> newContentItem, T appState)
        where T: IAppIdentity, IAppReadEntities, IAppReadContentTypes
    {
        Log.A($"create ent dic a#{appState.AppId}, type:{contentType}");
        // Retrieve content-type definition and check all the fields that this content-type has
        //var appState = Eav.Apps.State.Get(appId);
        var listOfTypes = appState.GetContentType(contentType);
        var attribs = listOfTypes.Attributes;

        var cleanedNewItem = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        foreach (var attrDef in attribs)
        {
            var attrName = attrDef.Name;
            if (!newContentItem.TryGetValue(attrName, out var foundValue) || foundValue == null)
                continue;
            switch (attrDef.Type)
            {
                case ValueTypes.String:
                case ValueTypes.Hyperlink:
                    if (foundValue is string)
                        cleanedNewItem.Add(attrName, foundValue.ToString()!);
                    else
                        throw ValueMappingError(attrDef, foundValue);
                    break;
                case ValueTypes.Boolean:
                    if (bool.TryParse(foundValue.ToString(), out var bolValue))
                        cleanedNewItem.Add(attrName, bolValue);
                    else
                        throw ValueMappingError(attrDef, foundValue);
                    break;
                case ValueTypes.DateTime:
                    if (DateTime.TryParse(foundValue.ToString(), out var dtm))
                        cleanedNewItem.Add(attrName, dtm);
                    else
                        throw ValueMappingError(attrDef, foundValue);
                    break;
                case ValueTypes.Number:
                    if (decimal.TryParse(foundValue.ToString(), out var dec))
                        cleanedNewItem.Add(attrName, dec);
                    else
                        throw ValueMappingError(attrDef, foundValue);
                    break;
                case ValueTypes.Entity:
                    var relationships = new List<int>();

                    void AddRelationshipIfNotNull(int? id) { if (id != null)
                        relationships.Add(id.Value); }
                    if (foundValue is IEnumerable foundEnum) // it's a list!
                        foreach (var item in foundEnum)
                            AddRelationshipIfNotNull(CreateSingleRelationshipItem(item));
                    else // not a list
                        AddRelationshipIfNotNull(CreateSingleRelationshipItem(foundValue));

                    cleanedNewItem.Add(attrName, relationships);

                    break;
                default:
                    throw new(
                        $"Tried to create fieldDef '{attrName}' but the type is not known: '{attrDef.Type}'");
            }

            // todo: maybe one day get default-values and insert them if not supplied by JS
        }

        AddPublishState(newContentItem, cleanedNewItem);

        return cleanedNewItem;
    }

    // add "PublishState" in "values" (before it can be removed when there is no "PublishState" fieldDef)
    private void AddPublishState(IDictionary<string, object?> values, IDictionary<string, object> cleaned)
    {
        if (!values.TryGetValue(SaveApiAttributes.SavePublishingState, out var value) || value == null)
            return;
        cleaned.Add(SaveApiAttributes.SavePublishingState, value);
    }

    /// <summary>
    /// In case of an error, show a nicer, consistent message
    /// </summary>
    /// <param name="fieldDef"></param>
    /// <param name="foundValue"></param>
    private static Exception ValueMappingError(IContentTypeField fieldDef, object foundValue)
        => new(
            $"Tried to create {fieldDef.Name} and couldn't convert to correct {fieldDef.Type}: '{foundValue}'");

    /// <summary>
    /// Takes input from JSON which could be in many formats like Category=ID or Category={id=#} 
    /// and then converts it to an item in the relationships-list
    /// </summary>
    /// <param name="foundValue"></param>
    private int? CreateSingleRelationshipItem(object foundValue)
    {
        var l = Log.Fn<int?>($"{foundValue}");
        try
        {
            // the object foundNumber is either just an Id, or an Id/Title combination
            // Try to see if it's already a number, else check if it's a JSON property
            if (int.TryParse(foundValue.ToString(), out var foundNumber)) 
                return l.ReturnAndLog(foundNumber);
                
            l.A("simple int-convert failed, try other formats");
            switch (foundValue)
            {
                case JsonElement jn when jn.ValueKind == JsonValueKind.Number:
                    return l.ReturnAndLog(jn.GetInt32());
                case JsonObject jo:
                {
                    l.A("Test cases where it's an object with 'id' property");

                    foreach (var key in new [] { "Id", "id" })
                        if (jo.TryGetPropertyValue(key, out var foundId))
                        {
                            l.A($"Found '{key}' - type {foundId?.GetType()} - string would be {foundId}");
                            return l.ReturnAndLog(foundId?.ToString().ConvertOrDefault<int?>());
                        }
                    break;
                }
            }
            return l.ReturnNull("not found/can't convert");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnNull("error");
        }
    }
}