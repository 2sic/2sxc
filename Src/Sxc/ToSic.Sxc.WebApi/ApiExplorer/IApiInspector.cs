using System.Collections.Generic;
using System.Reflection;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.WebApi.ApiExplorer
{
    public interface IApiInspector: IHasLog<IApiInspector>
    {
        bool IsBody(ParameterInfo paramInfo);

        List<string> GetHttpVerbs(MethodInfo methodInfo);
        
        ApiSecurityDto GetSecurity(MemberInfo member);
    }
}
