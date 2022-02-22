using System;
using System.Collections.Generic;
using ToSic.Eav.Context;

namespace IntegrationSamples.SxcEdit01.Context
{
    /// <summary>
    /// #2sxcIntegration
    /// Dummy user, which always says it's a superuser
    /// </summary>
    public class IntUserSuper: IUser
    {
        public int Id => 0;
        public string IdentityToken => "impl-user:0";
        public Guid? Guid => System.Guid.Empty;
        public List<int> Roles => new List<int>();
        public bool IsSuperUser => true;
        public bool IsAdmin => true;
        public bool IsDesigner => true;
        public bool IsAnonymous => false;
    }
}
