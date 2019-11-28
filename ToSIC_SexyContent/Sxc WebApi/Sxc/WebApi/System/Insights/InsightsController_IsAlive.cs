using System.Web.Http;
using ToSic.SexyContent.WebApi;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.System
{
    [SxcWebApiExceptionHandling]
    public partial class InsightsController
    {
        [HttpGet]
        public bool IsAlive()
        {
            ThrowIfNotSuperuser();
            return true;
        }
        

    }
}