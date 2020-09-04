using System;
using System.Collections.Generic;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcUser: IUser
    {
        public string IdentityToken => "mvcuser:1";
        public Guid? Guid => System.Guid.Empty;
        public List<int> Roles => new List<int>();

        // temp: set to true, so we can see better errors

        public bool IsSuperUser => true;
        public bool IsAdmin => false;
        public bool IsDesigner => false;
    }
}
