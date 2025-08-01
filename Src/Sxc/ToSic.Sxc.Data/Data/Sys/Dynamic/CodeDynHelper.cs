using System.Dynamic;
using ToSic.Eav.Data.Sys.Entities;
using ToSic.Sxc.Data.Sys.Decorators;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Data.Sys.Typed;

namespace ToSic.Sxc.Data.Sys.Dynamic;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CodeDynHelper(IEntity entity, SubDataFactory subDataFactory)
{
    public IEntity Entity { get; } = entity;
    public SubDataFactory SubDataFactory { get; } = subDataFactory;

    public IDynamicEntity? Presentation => _prs.Get(() => SubDataFactory.SubDynEntityOrNull(Entity.GetDecorator<EntityInBlockDecorator>()?.Presentation));
    private readonly GetOnce<IDynamicEntity?> _prs = new();

    public ITypedMetadata Metadata => _md.Get(() => SubDataFactory.Cdf.MetadataDynamic(Entity.Metadata))!;
    private readonly GetOnce<ITypedMetadata?> _md = new();
    public IDynamicEntity? Parent => _dp.Get(() => SubDataFactory.SubDynEntityOrNull(Entity.GetDecorator<EntityInBlockDecorator>()?.Parent));
    private readonly GetOnce<IDynamicEntity?> _dp = new();



    #region TryGetMember for dynamic access

    public static bool TryGetMemberAndRespectStrict(GetAndConvertHelper helper, GetMemberBinder binder, out object? result)
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