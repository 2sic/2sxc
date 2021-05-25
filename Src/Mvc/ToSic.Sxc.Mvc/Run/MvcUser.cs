using System;
using System.Collections.Generic;
using ToSic.Eav.Context;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcUser: IUser, ICmsUser
    {
        public int Id => 1;
        public new string IdentityToken => "mvcuser:1";
        public Guid? Guid => System.Guid.Empty;
        public List<int> Roles => new List<int>();
        public bool IsSuperUser => true;
        public bool IsAdmin => true;
        public bool IsDesigner => true;
        public bool IsAnonymous => false;

        #region New Permission properties for v12

        /// <inheritdoc />
        // This is a hopefully clearer implementation of what the user can do
        public bool IsSiteAdmin => IsAdmin;

        /// <inheritdoc />
        // This is a hopefully clearer implementation of what the user can do
        public bool IsSiteDeveloper => IsDesigner;

        /// <inheritdoc />
        // This is a hopefully clearer implementation of what the user can do
        public bool IsSystemAdmin => IsSuperUser;

        #endregion

    }
}
