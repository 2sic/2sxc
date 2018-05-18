using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Linq;
using System.Web.Http;
using ToSic.Eav.Apps;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.System
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    public partial class DebugController
    {

        [HttpGet]
        public string Attributes(int? appId = null, string type = null)
        {
            ThrowIfNotSuperuser();
            if (appId == null)
                return "please add appid to the url parameters";
            if (type == null)
                return "please add type to the url parameters";

            Log.Add($"debug app attributes for {appId} and {type}");
            var appRead = new AppRuntime(appId.Value, Log);
            var typ = appRead.ContentTypes.Get(type);
            //var pkg = appRead.Package;

            var msg = h1($"Attributes for {typ.Name} ({typ.StaticName}) in {appId}\n");
            try
            {
                Log.Add("getting content-type stats");
                var attribs = typ.Attributes;
                msg += p($"attribs: {attribs.Count}\n");
                msg += "<table id='table'><thead>" 
                    + tr(new[] { "#", "Id", "Name", "Type", "Input", "IsTitle", "Metadata" }, true)
                    + "</thead>"
                    + "<tbody>";
                var count = 0;
                foreach (var att in attribs)
                {
                    msg = msg + tr(new[] {
                        (++count).ToString(),
                        att.AttributeId.ToString(),
                        att.Name,
                        att.Type,
                        att.InputType,
                        att.IsTitle.ToString(),
                        att.Metadata.Count().ToString()
                          });
                }
                msg += "</tbody>";
                msg += "</table>";
                msg += "\n\n";
                msg += JsTableSort();
            }
            catch
            {
                // ignored
            }

            return msg;
        }
    }
}