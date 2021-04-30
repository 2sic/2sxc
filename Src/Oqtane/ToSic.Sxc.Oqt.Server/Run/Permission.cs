using System;
using System.Linq;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.Logging;
using ToSic.Sxc.Oqt.Server.Repository;
using ToSic.Sxc.Polymorphism;
using static System.StringComparison;


namespace ToSic.Sxc.Oqt.Server.Run
{
    [PolymorphResolver("Permissions")]
    public class Permissions : IResolver
    {
        private readonly Lazy<IUserResolver> _userResolver;
        private readonly Lazy<IUserRoleRepository> _userRoleRepository;
        private readonly Lazy<IRoleRepository> _roleRepository;
        public User User { get; private set;}
        public int? HostRoleId { get; private set; }
        public bool? HaveHost { get; private set; }

        public Permissions(Lazy<IUserResolver> userResolver, Lazy<IUserRoleRepository> userRoleRepository, Lazy<IRoleRepository> roleRepository)
        {
            _userResolver = userResolver;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
        }

        public string Name => "Permissions";

        public const string ModeIsSuperUser = "IsSuperUser";

        public string Edition(string parameters, ILog log)
        {
            var wrapLog = log.Call<string>();
            if (!string.Equals(parameters, ModeIsSuperUser, InvariantCultureIgnoreCase))
                return wrapLog("unknown param", null);
            var isSuper = IsHost();
            var result = isSuper ? "staging" : "live";
            return wrapLog(result, result);
        }

        public bool IsHost()
        {
            User ??= _userResolver.Value.GetUser();
            HostRoleId ??= _roleRepository.Value.GetRoles(User.SiteId, true)
                .FirstOrDefault(item => item.Name == RoleNames.Host).RoleId;
            HaveHost ??= _userRoleRepository.Value.GetUserRoles(User.UserId, User.SiteId)
                .Any(r => r.RoleId == HostRoleId);
            return HaveHost ?? false;
        }
    }
}