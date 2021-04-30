using System;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Polymorphism
{
    internal class ResolverInfo: TypeWithMedataBase<PolymorphResolverAttribute>
    {
        public ResolverInfo(Type dsType): base (dsType)
        {
            //Type = dsType;

            // must put this in a try/catch, in case other DLLs have incompatible attributes
            //try
            //{
            //    var info = Type.GetCustomAttributes(typeof(PolymorphResolverAttribute), false).FirstOrDefault() as
            //            PolymorphResolverAttribute;

            //}

            //catch {  /*ignore */ }
            //Name = TypeMetadata.Name;
        }

        public override string Name => TypeMetadata?.Name;

        //public Type Type { get; }

    }
}
