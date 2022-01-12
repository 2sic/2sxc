using System;
using ToSic.Eav.Documentation;

// Unsure if used! maybe remove??? 

// TODO: @STV - find out if this is in use / needed, if yes, move to Plumbing folder, otherwise delete

namespace ToSic.Sxc.Oqt.Server.Engines
{
    /// <summary>
    /// Dummy class - just to ensure that the razor compiler is included in the build
    /// </summary>
    [PrivateApi]
    public class DependencyOnRazorCompiler
    {
        public static bool Dummy(Type compare)
        {
            Action<Type> noop = _ => { };
            var dummy = typeof(Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation.FileProviderRazorProjectItem);
            noop(dummy);
            if (compare == dummy) return true;

            var dummy2 = typeof(Microsoft.AspNetCore.Razor.Language.AllowedChildTagDescriptor);
            noop(dummy2);
            if (compare == dummy2) return true;

            var dummy3 = typeof(Microsoft.AspNetCore.Antiforgery.AntiforgeryOptions);
            noop(dummy3);
            if (compare == dummy3) return true;

            return false;
        }
    }
}
