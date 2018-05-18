using System.Web.Http;
using System.Web.Http.Controllers;
using ToSic.SexyContent.WebApi;
using ToSic.SexyContent.WebApi.Dnn;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.System
{
    [SxcWebApiExceptionHandling]
    public partial class InsightsController : DnnApiControllerWithFixes
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("Api.Debug");
        }
        
        [HttpGet]
        public bool IsAlive()
        {
            ThrowIfNotSuperuser();
            return true;
        }
        

    }
}