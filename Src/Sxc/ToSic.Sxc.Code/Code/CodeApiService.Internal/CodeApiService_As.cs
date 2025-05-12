using ToSic.Lib.Helpers;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Services;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;

namespace ToSic.Sxc.Code.Internal;

public partial class CodeApiService
{
    [PrivateApi]
    public ICodeDataFactory Cdf => _cdf.Get(() =>
    {
        Services.Cdf.ConnectToRoot(this);
        return Services.Cdf;
    });
    private readonly GetOnce<ICodeDataFactory> _cdf = new();

    #region AsDynamic Implementations

    /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
    public dynamic AsDynamic(string json, string fallback = default) => Cdf.Json2Jacket(json, fallback);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
    public dynamic AsDynamic(IEntity entity) => Cdf.CodeAsDyn(entity);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(object)" />
    public dynamic AsDynamic(object dynamicEntity) => Cdf.AsDynamicFromObject(dynamicEntity);

    // 2025-05-11 2dm disabled since we're not supporting DynamicCode12 any more...
    ///// <inheritdoc cref="IDynamicCode12.AsDynamic(object[])" />
    //public dynamic AsDynamic(params object[] entities) => Cdf.MergeDynamic(entities);


    /// <inheritdoc cref="IDynamicCode.AsEntity" />
    public IEntity AsEntity(object dynamicEntity) => Cdf.AsEntity(dynamicEntity);

    #endregion

    #region AsList

    /// <inheritdoc cref="IDynamicCode.AsList" />
    public IEnumerable<dynamic> AsList(object list) => Cdf.CodeAsDynList(list);

    #endregion

    #region Convert

    /// <inheritdoc />
    public IConvertService Convert => field ??= Services.ConvertService.Value;

    #endregion

    #region Adam

    /// <inheritdoc cref="IDynamicCode.AsAdam" />
    public IFolder AsAdam(ICanBeEntity item, string fieldName) 
        => Cdf.Folder(item, fieldName, null);

    #endregion
}