using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Polymorphism.Internal;

internal class ResolverInfo(Type dsType) : TypeWithMetadataBase<PolymorphResolverAttribute>(dsType)
{
    public override string Name => TypeMetadata?.Name;
}