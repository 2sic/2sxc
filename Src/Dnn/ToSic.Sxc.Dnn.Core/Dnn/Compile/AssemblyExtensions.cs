using System.Reflection;
using System.Web.Configuration;

namespace ToSic.Sxc.Dnn.Compile
{
    public static class AssemblyExtensions
    {
        // for assembly references
        public static Assembly WithPolicy(this Assembly assembly) 
            =>
            // apply binding redirections from web.config
            Assembly.ReflectionOnlyLoad(System.AppDomain.CurrentDomain.ApplyPolicy(assembly.FullName));

        public static Assembly WithPolicy(this AssemblyInfo ai)
            =>
                // apply binding redirections from web.config
                Assembly.ReflectionOnlyLoad(System.AppDomain.CurrentDomain.ApplyPolicy(ai.Assembly));
    }
}
