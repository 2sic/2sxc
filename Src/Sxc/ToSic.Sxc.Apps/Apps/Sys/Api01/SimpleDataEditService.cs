﻿using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.Entities;
using ToSic.Eav.Data.Sys.EntityPair;
using ToSic.Eav.Data.Sys.Save;
using ToSic.Eav.Metadata;
using static System.StringComparer;


// This is the simple API used to quickly create/edit/delete entities

// todo: there is quite a lot of duplicate code here
// like code to build attributes, or convert id-relationships to guids
// this should be in the AttributeBuilder or similar

namespace ToSic.Sxc.Apps.Sys.Api01;

/// <summary>
/// This is a simple controller with some Create, Update and Delete commands.
///
/// Used to be called SimpleDataController before v17
/// </summary>
/// <remarks>
/// Used for DI - must always call Init to use
/// </remarks>
[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class SimpleDataEditService(
    DataBuilder builder,
    IZoneMapper zoneMapper,
    IContextOfSite ctx,
    GenWorkDb<WorkEntitySave> entSave,
    GenWorkDb<WorkEntityUpdate> entUpdate,
    GenWorkDb<WorkEntityDelete> entDelete,
    Generator<AppPermissionCheck> appPermissionCheckGenerator) : ServiceBase("Dta.Simple", connect: [entSave, entUpdate, entDelete, zoneMapper, builder, ctx, appPermissionCheckGenerator])
{

    #region Constructor / DI


    private string _defaultLanguageCode = null!;

    private int _appId;
    private bool _checkWritePermissions = true; // default behavior is to check write publish/draft permissions (that should happen for REST, but not for c# API)
    private IAppWorkCtxWithDb _ctxWithDb = null!;

    /// <param name="zoneId">Zone ID</param>
    /// <param name="appId">App ID</param>
    /// <param name="checkWritePermissions"></param>
    public SimpleDataEditService Init(int zoneId, int appId, bool checkWritePermissions = true)
    {
        var l = Log.Fn<SimpleDataEditService>($"{zoneId}, {appId}");
        _appId = appId;

        // when zoneId is not that same as in current context, we need to set right site for provided zoneId
        if (ctx.Site.ZoneId != zoneId)
            ctx.Site = zoneMapper.SiteOfZone(zoneId);

        _defaultLanguageCode = GetDefaultLanguage(zoneId);
        var appIdentity = new AppIdentity(zoneId, appId);
        _ctxWithDb = entSave.CtxSvc.CtxWithDb(appIdentity);
        _checkWritePermissions = checkWritePermissions;
        l.A($"Default language:{_defaultLanguageCode}");
        return l.Return(this);
    }

    private string GetDefaultLanguage(int zoneId)
    {
        var l = Log.Fn<string>($"{zoneId}");
        var site = zoneMapper.SiteOfZone(zoneId);
        if (site == null! /* paranoid */)
            return l.Return("", "site is null");

        var usesLanguages = zoneMapper.CulturesEnabledWithState(site).Any();
        return l.Return(usesLanguages ? site.DefaultCultureCode : "", $"ok, usesLanguages:{usesLanguages}");
    }
        
    #endregion

    /// <summary>
    /// Create a new entity of the content-type specified.
    /// </summary>
    /// <param name="contentTypeName">Content-type</param>
    /// <param name="multiValues">
    ///     Values to be set collected in a dictionary. Each dictionary item is a pair of attribute 
    ///     name and value. To set references to other entities, set the attribute value to a list of 
    ///     entity ids. 
    /// </param>
    /// <param name="target"></param>
    /// <exception cref="ArgumentException">Content-type does not exist, or an attribute in attributes</exception>
    public IEnumerable<int> Create(string contentTypeName, IEnumerable<Dictionary<string, object?>>? multiValues, ITarget? target = null) 
    {
        var list = multiValues?.ToListOpt();
        var l = Log.Fn<IEnumerable<int>>($"{contentTypeName}, items: {list?.Count()}, target: {target != null}");
        if (list == null)
            return l.Return([],"attributes were null");

        // ensure the type really exists - will throw if not
        var type = _ctxWithDb.AppReader.GetContentType(contentTypeName);

        l.A($"Type {contentTypeName} found. Will build entities to save...");

        var entSaver = entSave.New(_ctxWithDb.AppReader);
        var saveOptions = entSaver.SaveOptions();

        var importEntity = list
            .Select(values =>
            {
                var (entity, publishing) = BuildNewEntity(type, values, target, null);
                var mySaveOptions = saveOptions with { DraftShouldBranch = publishing.ShouldBranchDrafts };
                var pair = new EntityPair<SaveOptions>(entity, mySaveOptions);
                return pair;
            })
            .ToList();

        var ids = entSaver
            .Save(importEntity);

        var intIds = ids
            .Select(i => i.Id)
            .ToListOpt();

        return l.Return(intIds, "ok");
    }

    private (IEntity Entity, EntitySavePublishing Publishing) BuildNewEntity(
        IContentType type,
        Dictionary<string, object?> values,
        ITarget? targetOrNull,
        bool? existingIsPublished) 
    {
        var l = Log.Fn<(IEntity Entity, EntitySavePublishing Publishing)>
            ($"{type.Name}, {values.Count}, target: {targetOrNull != null}; {existingIsPublished}");
        // We're going to make changes to the dictionary, so we MUST copy it first, so we don't affect upstream code
        // also ensure its case-insensitive...
        values = values.ToInvIgnoreCaseCopy();

        Guid eGuid;
        if (values.TryGetValue(AttributeNames.EntityFieldGuid, out var entityGuidString))
        {
            // Note: we want an error if this guid is wrong, because we can't just take a new one if we got one
            // since upstream may later want to access the same entity
            eGuid = Guid.Parse(entityGuidString?.ToString() ?? "");
        }
        else
        {
            l.A("Add new generated guid, as none was provided.");
            eGuid = Guid.NewGuid();
            values.Add(AttributeNames.EntityFieldGuid, eGuid);
        }


        // Get owner form value dictionary (and remove it from attributes) because we need to provided it in entity constructor.
        string? owner = null;
        if (values.TryGetValue(AttributeNames.EntityFieldOwner, out var ownerObj))
        {
            l.A("Get owner, when is provided.");
            owner = ownerObj?.ToString();
            values.Remove(AttributeNames.EntityFieldOwner);
        }

        // Figure out publishing before converting to IAttribute
        var publishing = DetectPublishingOrError(type, values, existingIsPublished);

        // Prepare attributes to add
        var preparedValues = ConvertRelationsToNullArray(type, values);
        var preparedIAttributes = builder.Attribute.Create(preparedValues);
        var attributes = BuildNewEntityValues(type, preparedIAttributes, _defaultLanguageCode);

        var newEntity = builder.Entity.Create(
            appId: _appId,
            guid: eGuid,
            contentType: type,
            attributes: builder.Attribute.Create(attributes),
            owner: owner,
            metadataFor: targetOrNull,
            isPublished: publishing.ShouldPublish);
        if (targetOrNull != null)
            l.A("FYI: Set metadata target which was provided.");

        return l.Return((newEntity, publishing));
    }


    /// <summary>
    /// Update an entity specified by ID.
    /// </summary>
    /// <param name="entityId">Entity ID</param>
    /// <param name="values">
    ///     Values to be set collected in a dictionary. Each dictionary item is a pair of attribute 
    ///     name and value. To set references to other entities, set the attribute value to a list of 
    ///     entity ids. 
    /// </param>
    /// <exception cref="ArgumentException">Attribute in attributes does not exit</exception>
    /// <exception cref="ArgumentNullException">Entity does not exist</exception>
    public void Update(int entityId, Dictionary<string, object?> values)
    {
        var l = Log.Fn($"update i:{entityId}");
        var original = _ctxWithDb.AppReader.List.FindRepoId(entityId)
            ?? throw new NullReferenceException($"Can't Update, original not found with ID {entityId}");
        var (entity, publishing) = BuildNewEntity(original.Type, values, null, original.IsPublished);
        entUpdate.New(_ctxWithDb.AppReader)
            .UpdateParts(id: entityId, partialEntity: entity, publishing: publishing);
        l.Done();
    }


    /// <summary>
    /// Delete the entity specified by ID.
    /// </summary>
    /// <param name="entityId">Entity ID</param>
    /// <exception cref="InvalidOperationException">Entity cannot be deleted for example when it is referenced by another object</exception>
    public void Delete(int entityId) => entDelete.New(_ctxWithDb.AppReader).Delete(entityId);


    /// <summary>
    /// Delete the entity specified by GUID.
    /// </summary>
    /// <param name="entityGuid">Entity GUID</param>
    public void Delete(Guid entityGuid) => entDelete.New(_ctxWithDb.AppReader).Delete(entityGuid);


    private IDictionary<string, object?> ConvertRelationsToNullArray(IContentType contentType, IDictionary<string, object?> values)
    {
        var l = Log.Fn<IDictionary<string, object?>>();
        // Find all attributes which are relationships
        var relationships = contentType.Attributes
            .Where(a => a.Type == ValueTypes.Entity)
            .ToListOpt();

        var newValues = values
            .ToDictionary(
                pair => pair.Key,
                pair =>
                {
                    var value = pair.Value;
                    // Not relationship, don't convert
                    if (!relationships.Any(a => a.Name.EqualsInsensitive(pair.Key)))
                        return value;

                    switch (value)
                    {
                        case null: return null;
                        case int intVal:
                            return new List<int?> { intVal };
                        case Guid guidVal:
                            return new List<Guid?> { guidVal };
                        case IEnumerable<int> idInt:
                            return idInt.Cast<int?>().ToList();
                        case IEnumerable<int?> idIntNull:
                            return idIntNull.ToList();
                        case IEnumerable<Guid> idGuid:
                            return idGuid.Cast<Guid?>().ToList();
                        case IEnumerable<Guid?> idGuidNull:
                            return idGuidNull.ToList();
                        case string strValEmpty when !strValEmpty.HasValue():
                            return null;
                        case string strVal:
                            var parts = strVal.CsvToArrayWithoutEmpty();
                            if (parts.Length == 0)
                                return value;

                            // could be int/guid - must convert - must all be the same
                            if (int.TryParse(parts[0], out var intValue))
                                return parts
                                    .Select(item => int.TryParse(item, out intValue) ? (int?)intValue : null)
                                    .ToList();

                            if (Guid.TryParse(parts[0], out var guidValue))
                                return parts
                                    .Select(item => Guid.TryParse(item, out guidValue) ? (Guid?)guidValue : null)
                                    .ToList();

                            // fallback
                            return value;
                        default:
                            return value;
                    }
                });
        return l.Return(newValues);
    }


    private IDictionary<string, IAttribute> BuildNewEntityValues(IContentType contentType, IReadOnlyDictionary<string, IAttribute> attributes, string valuesLanguage)
    {
        var l = Log.Fn<Dictionary<string, IAttribute>>($"..., ..., attributes: {attributes.Count}, {valuesLanguage}");
        if (attributes.SafeNone())
            return l.Return(new(), "null/empty");

        var updated = attributes
            .Select(keyValuePair =>
            {
                // Handle content-type attributes
                var ctAttr = contentType[keyValuePair.Key];
                if (ctAttr == null || keyValuePair.Value == null)
                    return null;

                attributes.TryGetValue(ctAttr.Name, out var attribute);
                var firstValue = keyValuePair.Value.Values?.FirstOrDefault();
                var firstValContents = firstValue?.ObjectContents;
                if (firstValContents == null)
                    return null;
                var preConverted =
                    builder.Value.PreConvertReferences(firstValContents, ctAttr.Type, true);
                var newAttribute = builder.Attribute.CreateOrUpdate(
                    originalOrNull: attribute,
                    name: ctAttr.Name,
                    value: preConverted,
                    type: ctAttr.Type,
                    valueToReplace: firstValue,
                    language: valuesLanguage);
                l.A($"Attribute '{keyValuePair.Key}' will become '{keyValuePair.Value}' ({ctAttr.Type})");
                return new
                {
                    keyValuePair.Key,
                    Attribute = newAttribute
                };

            })
            .Where(x => x != null)
            .ToDictionary(
                pair => pair!.Key,
                pair => pair!.Attribute,
                InvariantCultureIgnoreCase
            );
        return l.Return(updated, "done");
    }
    
}