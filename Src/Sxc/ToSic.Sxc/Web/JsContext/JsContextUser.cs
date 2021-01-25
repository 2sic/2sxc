﻿using ToSic.Eav.Context;

namespace ToSic.Sxc.Web.JsContext
{
    public class JsContextUser
    {
        public bool CanDesign;
        public bool CanDevelop;

        public JsContextUser(IUser user)
        {
            CanDesign = user.IsDesigner;
            CanDevelop = user.IsSuperUser;
        }
    }
}
