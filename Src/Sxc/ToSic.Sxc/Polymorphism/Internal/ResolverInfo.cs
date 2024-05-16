using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Polymorphism.Internal;

/// <summary>
/// Type - mainly name - of a resolver
/// </summary>
/// <param name="dsType"></param>
internal class ResolverInfo(Type dsType) : TypeWithMetadataBase<PolymorphResolverAttribute>(dsType)
{
    public override string NameId => TypeMetadata?.Name;
}