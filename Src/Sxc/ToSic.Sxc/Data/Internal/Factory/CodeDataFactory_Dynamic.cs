using System.Dynamic;
using ToSic.Eav.DataSource;
using ToSic.Sxc.Data.Internal.Wrapper;

namespace ToSic.Sxc.Data.Internal;

partial class CodeDataFactory
{

    #region Dynamic

    /// <summary>
    /// Implement AsDynamic for DynamicCode - not to be used in internal APIs.
    /// Always assumes Strict is false
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public DynamicEntity CodeAsDyn(IEntity entity)
        => new(entity, this, propsRequired: false);

    public DynamicEntity AsDynamic(IEntity entity, bool propsRequired) =>
        new(entity, this, propsRequired: propsRequired);

    /// <summary>
    /// Convert a list of Entities into a DynamicEntity.
    /// Only used in DynamicCodeRoot.
    /// </summary>
    internal DynamicEntity AsDynamicFromEntities(IEnumerable<IEntity> list, bool propsRequired) 
        => new(list: list, parent: null, field: null, appIdOrNull: null, propsRequired: propsRequired, cdf: this);

    /// <summary>
    /// Convert any object into a dynamic list.
    /// Only used in Dynamic Code for the public API.
    /// </summary>
    public IEnumerable<dynamic> CodeAsDynList(object list, bool propsRequired = false)
    {
        switch (list)
        {
            case null:
                return new List<dynamic>();
            case IDataSource dsEntities:
                return CodeAsDynList(dsEntities.List);
            case IEnumerable<IEntity> iEntities:
                return iEntities.Select(e => AsDynamic(e, propsRequired: propsRequired));
            case IEnumerable<IDynamicEntity> dynIDynEnt:
                return dynIDynEnt;
            case IEnumerable<dynamic> dynEntities:
                return dynEntities;
            default:
                return null;
        }
    }


    /// <summary>
    /// Convert any object into a dynamic object.
    /// Only used in Dynamic Code for the public API.
    /// </summary>
    public object AsDynamicFromObject(object dynObject, bool propsRequired = false)
    {
        var l = Log.Fn<object>();
        //var typed = AsTypedInternal(dynObject);
        //if (typed != null) return l.Return(typed, nameof(ITypedRead));

        switch (dynObject)
        {
            case null:
                return l.Return(Json2Jacket(null), "null");
            case string strObject:
                return l.Return(Json2Jacket(strObject), "string");
            case IDynamicEntity dynEnt:
                return l.Return(dynEnt, "DynamicEntity");
            // New case - should avoid re-converting dynamic json, DynamicStack etc.
            case ISxcDynamicObject sxcDyn:
                return l.Return(sxcDyn, "Dynamic Something");
            case IEntity entity:
                return l.Return(new DynamicEntity(entity, this, propsRequired: propsRequired), "IEntity");
            case DynamicObject typedDynObject:
                return l.Return(typedDynObject, "DynamicObject");
            default:
                // Check value types - note that it won't catch strings, but these were handled above
                if (dynObject.GetType().IsValueType) return l.Return(dynObject, "bad call - value type");

                // 2021-09-14 new - just convert to a DynamicReadObject
                var result = codeDataWrapper.Value.ChildNonJsonWrapIfPossible(data: dynObject,
                    // 2023-08-08 2dm - changed `wrapNonAnon` to true, I'm not sure why it was false, but I'm certain that's wrong
                    wrapNonAnon: true /* false, */,
                    WrapperSettings.Dyn(children: true, realObjectsToo: false));
                if (result is not null) return l.Return(result, "converted to dyn-read");

                // Note 2dm 2021-09-14 returning the original object was actually the default till now.
                // Unknown conversion, just return the original and see what happens/breaks
                // probably not a good solution
                return l.Return(dynObject, "unknown, return original");
        }
    }

    #endregion


    #region Merge Dynamic

    public dynamic MergeDynamic(object[] entities)
    {
        if (entities == null || !entities.Any()) return null;
        // 2023-08-08 2dm disable this 1-only optimization, because it results in a slightly different object
        // if (entities.Length == 1) return AsDynamicFromObject(entities[0]);
        return AsStack(null, entities, strictTypes: false, AsDynStack);
        //return AsStack(entities);
    }

    #endregion

}