using System;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Polymorphism;

internal class ResolverInfo: TypeWithMetadataBase<PolymorphResolverAttribute>
{
    public ResolverInfo(Type dsType): base (dsType)
    {

    }

    public override string Name => TypeMetadata?.Name;
}