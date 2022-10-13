using System;
using System.Collections.Generic;
using ToSic.Eav.Context;

namespace IntegrationSamples.BasicEav01.Context
{
    /// <summary>
    /// #2sxcIntegration
    /// Dummy user, which always says it's a superuser
    /// </summary>
    public class IntUser: IUser
    {
        public int Id => 0;
        public string IdentityToken => "impl-user:0";
        public Guid? Guid => System.Guid.Empty;
        public List<int> Roles => new List<int>();
        public bool IsSystemAdmin => true;
        [Obsolete("deprecated in v14.09 2022-10, will be removed ca. v16 #remove16")]
        public bool IsSuperUser => true;
        [Obsolete("deprecated in v14.09 2022-10, will be removed ca. v16 #remove16")]
        public bool IsAdmin => IsSiteAdmin;

        public bool IsSiteAdmin => true;
        public bool IsContentAdmin => true;
        public bool IsDesigner => true;
        public bool IsAnonymous => false;

        public string Username => "unknown";

        public string Name => Username;

        public string Email => "unknown@unknown.org";

    }
}
