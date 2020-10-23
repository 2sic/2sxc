using Microsoft.AspNetCore.Http;
using Oqtane.Models;
using Oqtane.Modules;
using System.Linq;
using System.Security.Claims;

namespace ToSic.Sxc.OqtaneModule.Server.Repository
{
    class UserResolver : IUserResolver, IService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public User GetUser()
        {
            var user = new User { IsAuthenticated = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated, Username = "", UserId = -1, Roles = "" };
            if (!user.IsAuthenticated) return user;

            user.Username = _httpContextAccessor.HttpContext.User.Identity.Name;
            user.UserId = int.Parse(_httpContextAccessor.HttpContext.User.Claims.First(item => item.Type == ClaimTypes.PrimarySid).Value);
            var roles = _httpContextAccessor.HttpContext.User.Claims.Where(item => item.Type == ClaimTypes.Role).Aggregate("", (current, claim) => current + (claim.Value + ";"));
            if (roles != "") roles = ";" + roles;
            user.Roles = roles;
            return user;
        }
    }
}
