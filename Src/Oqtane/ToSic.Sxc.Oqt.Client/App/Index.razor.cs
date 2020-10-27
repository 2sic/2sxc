using Oqtane.Models;
using Oqtane.UI;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ToSic.Sxc.Oqt.App
{
    public partial class Index
    {
        public string ToSxcModulePath()
        {
            return "Modules/ToSic.Sxc/";
        }

        public override List<Resource> Resources
        {
            get
            {
                return new List<Resource>();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            // prepare the html / headers
            SxcEngine.Prepare(PageState.Site, PageState.Page, ModuleState);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await base.OnAfterRenderAsync(firstRender);

                if (PageState.Runtime == Runtime.Server)
                {
                    var interop = new Interop(JSRuntime);

                    // HACK: Lets load all 2sxc js dependencies (js / styles)
                    foreach (var resource in SxcEngine.AssetsAndHeaders.Scripts())
                        await interop.IncludeScript("", resource, "", "", "", "head", "");

                    foreach (var style in SxcEngine.AssetsAndHeaders.Styles())
                        await interop.IncludeLink("", "stylesheet", style, "text/css", "", "", ""); //.IncludeScript("", resource.Url, resource.Integrity ?? "", resource.CrossOrigin ?? "", "", "head", "");

                    var aAndH = SxcEngine.AssetsAndHeaders;
                    if (aAndH.AddContextMeta)
                        await interop.IncludeMeta("sxc-tmp-context-id", "name", aAndH.ContextMetaName, aAndH.ContextMetaContents(), "id");

                    //await interop.IncludeMeta("kk1", "name", "description", $"content {PageState.Page.PageId} {PageState.ModuleId}", "id");
                    //await interop.IncludeMeta("kk2", "name", "keywords", $"content {PageState.Page.PageId} {PageState.ModuleId}", "id");
                    //await interop.IncludeMeta("kk3", "name", "author", $"content {PageState.Page.PageId} {PageState.ModuleId}", "id");

                    string antiforgerytoken = await interop.GetElementByName("__RequestVerificationToken");
                    //var fields = new { __RequestVerificationToken = antiforgerytoken, username = _username, password = _password, remember = _remember, returnurl = _returnUrl };
                    //await interop.IncludeLink("app-stylesheet", "stylesheet", "https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css", "text/css", "sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T", "anonymous", "");
                    //await interop.IncludeScripts(scripts.ToArray());
                }
            }
        }
    }
}
