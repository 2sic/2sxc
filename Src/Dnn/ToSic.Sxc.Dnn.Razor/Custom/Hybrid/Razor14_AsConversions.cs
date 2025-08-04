using ToSic.Sxc.Adam;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

abstract partial class Razor14
{
    #region AsDynamic in many variations

    /// <inheritdoc cref="IDynamicCodeDocs.AsDynamic(string, string)" />
    public dynamic AsDynamic(string json, string fallback = default) => CodeApi.Cdf.Json2Jacket(json, fallback);

    /// <inheritdoc cref="IDynamicCodeDocs.AsDynamic(IEntity)" />
    public dynamic AsDynamic(IEntity entity) => CodeApi.Cdf.CodeAsDyn(entity);

    /// <inheritdoc cref="IDynamicCodeDocs.AsDynamic(string, string)" />
    public dynamic AsDynamic(object dynamicEntity) => CodeApi.Cdf.AsDynamicFromObject(dynamicEntity);

    /// <inheritdoc cref="IDynamicCode12Docs.AsDynamic(object[])" />
    public dynamic AsDynamic(params object[] entities) => CodeApi.Cdf.MergeDynamic(entities);

    #endregion

    #region AsEntity
    /// <inheritdoc cref="IDynamicCodeDocs.AsEntity" />
    public IEntity AsEntity(object dynamicEntity) => CodeApi.Cdf.AsEntity(dynamicEntity);
    #endregion

    #region AsList

    /// <inheritdoc cref="IDynamicCodeDocs.AsList" />
    public IEnumerable<dynamic> AsList(object list) => CodeApi.Cdf.CodeAsDynList(list);

    #endregion

    /// <inheritdoc cref="IDynamicCodeDocs.AsAdam" />
    public IFolder AsAdam(ICanBeEntity item, string fieldName) => CodeApi.AsAdam(item, fieldName);

}