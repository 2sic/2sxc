using ToSic.Lib.Helpers;
using ToSic.Sxc.Services;
using CodeDataFactory = ToSic.Sxc.Data.Internal.CodeDataFactory;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;

namespace ToSic.Sxc.Code.Internal;

public partial class CodeApiService
{
    [PrivateApi]
    public CodeDataFactory _Cdf => _cdf.Get(() =>
    {
        Services.Cdf.ConnectToRoot(this);
        return Services.Cdf;
    });
    private readonly GetOnce<CodeDataFactory> _cdf = new();

    #region AsDynamic Implementations

    /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
    public dynamic AsDynamic(string json, string fallback = default) => _Cdf.Json2Jacket(json, fallback);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
    public dynamic AsDynamic(IEntity entity) => _Cdf.CodeAsDyn(entity);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(object)" />
    public dynamic AsDynamic(object dynamicEntity) => _Cdf.AsDynamicFromObject(dynamicEntity);

    /// <inheritdoc cref="IDynamicCode12.AsDynamic(object[])" />
    public dynamic AsDynamic(params object[] entities) => _Cdf.MergeDynamic(entities);


    /// <inheritdoc cref="IDynamicCode.AsEntity" />
    public IEntity AsEntity(object dynamicEntity) => _Cdf.AsEntity(dynamicEntity);

    #endregion

    #region AsList

    /// <inheritdoc cref="IDynamicCode.AsList" />
    public IEnumerable<dynamic> AsList(object list) => _Cdf.CodeAsDynList(list);

    #endregion

    #region Convert

    /// <inheritdoc />
    public IConvertService Convert => _convert ??= Services.ConvertService.Value;
    private IConvertService _convert;

    #endregion

    #region Adam

    /// <inheritdoc cref="IDynamicCode.AsAdam" />
    public IFolder AsAdam(ICanBeEntity item, string fieldName) 
        => _Cdf.Folder(item, fieldName, null);

    #endregion
}