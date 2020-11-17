using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

[assembly: ProvideApplicationPartFactory(typeof(ToSic.Sxc.Oqt.Server.Plumbing.MyApplicationPartFactory))]
namespace ToSic.Sxc.Oqt.Server.Plumbing
{
    public class MyApplicationPartFactory : ApplicationPartFactory
    {
        public override IEnumerable<ApplicationPart> GetApplicationParts(Assembly assembly)
        {
            yield return new CompilationReferencesProvider(assembly);
        }
    }
}
