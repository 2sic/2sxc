using System.Dynamic;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data.Internal.Decorators;
using ToSic.Sxc.Data.Internal.Typed;

namespace ToSic.Sxc.Data.Internal.Dynamic;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CodeDynHelper(IEntity entity, SubDataFactory subDataFactory)
{
    public IEntity Entity { get; } = entity;
    public SubDataFactory SubDataFactory { get; } = subDataFactory;

    public IDynamicEntity Presentation => _prs.Get(() => SubDataFactory.SubDynEntityOrNull(Entity.GetDecorator<EntityInBlockDecorator>()?.Presentation));
    private readonly GetOnce<IDynamicEntity> _prs = new();

    public IMetadata Metadata => _md.Get(() => SubDataFactory.Cdf.Metadata(Entity?.Metadata));
    private readonly GetOnce<IMetadata> _md = new();
    public IDynamicEntity Parent => _dp.Get(() => SubDataFactory.SubDynEntityOrNull(Entity.GetDecorator<EntityInBlockDecorator>()?.Parent));
    private readonly GetOnce<IDynamicEntity> _dp = new();



    #region TryGetMember for dynamic access

    public static bool TryGetMemberAndRespectStrict(GetAndConvertHelper helper, GetMemberBinder binder, out object result)
    {
        var findResult = helper.GetInternal(binder.Name, lookupLink: true);
        // ReSharper disable once ExplicitCallerInfoArgument
        if (!findResult.Found && helper.PropsRequired)
            throw TypedHelpers.ErrStrict(binder.Name, cName: ".");
        result = findResult.Result;
        return true;
    }

    #endregion

}