using System.Dynamic;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSource;
using ToSic.Sxc.Data.Internal.Wrapper;

namespace ToSic.Sxc.Data.Internal;

partial class CodeDataFactory: ICodeDataFactoryDeepWip
{

    #region Dynamic

    /// <summary>
    /// Implement AsDynamic for DynamicCode - not to be used in internal APIs.
    /// Always assumes Strict is false
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public IDynamicEntity CodeAsDyn(IEntity entity)
        => new DynamicEntity(entity, this, propsRequired: false);

    public IDynamicEntity AsDynamic(IEntity entity, bool propsRequired) =>
        new DynamicEntity(entity, this, propsRequired: propsRequired);

    /// <summary>
    /// Convert a list of Entities into a DynamicEntity.
    /// Only used in DynamicCodeRoot.
    /// </summary>
    public IDynamicEntity AsDynamicFromEntities(IEnumerable<IEntity> list, bool propsRequired, NoParamOrder protector = default, IEntity parent = default, string field = default) 
        => new DynamicEntity(list: list, parent: parent, field: field, appIdOrNull: null, propsRequired: propsRequired, cdf: this);

    /// <summary>
    /// Convert any object into a dynamic list.
    /// Only used in Dynamic Code for the public API.
    /// </summary>
    public IEnumerable<dynamic> CodeAsDynList(object list, bool propsRequired = false) =>
        list switch
        {
            null => new List<dynamic>(),
            IDataSource dsEntities => CodeAsDynList(dsEntities.List),
            IEnumerable<IEntity> iEntities => iEntities.Select(e => AsDynamic(e, propsRequired: propsRequired)),
            IEnumerable<IDynamicEntity> dynIDynEnt => dynIDynEnt,
            IEnumerable<dynamic> dynEntities => dynEntities,
            _ => null
        };


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

    public dynamic MergeDynamic(object[] entities) =>
        entities == null || !entities.Any()
            ? null
            : AsStack(null, entities, strictTypes: false, AsDynStack);

    #endregion

    // 2026-05-13 2dm - not sure if AppReaderRequired is correct, but I assume that if we do have a context,
    // then it must have an AppReader, so we can use it.
    public IAppReader AppReaderOrNull => BlockOrNull?.Context?.AppReaderRequired;

    int ICodeDataFactoryDeepWip.AppIdOrZero => BlockOrNull?.AppId ?? 0;
}