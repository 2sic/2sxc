using System.IO;
using ToSic.Eav.Helpers;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Sxc.Apps.Paths;
using ToSic.Sxc.Code;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Razor
{
    public class RazorService : IRazorService
    {

        private readonly LazySvc<IRazorRenderer> _razorRendererLazy;

        public RazorService(LazySvc<IRazorRenderer> razorRendererLazy)
        {
            _razorRendererLazy = razorRendererLazy;
        }

        /// <summary>
        /// render razorPage
        /// </summary>
        /// <param name="partialName">virtual path to razorPage from 2sxc app root folder</param>
        /// <param name="model">data</param>
        /// <returns>string</returns>
        public string Render(string partialName, object model = null)
        {
            var razorPageVirtualPath = GetVirtualPath(partialName);
            var razorPageModel = model ?? new {};

            var task = _razorRendererLazy.Value.RenderToStringAsync(razorPageVirtualPath, razorPageModel, null, GetAppCodePath());
            task.Wait();
            return task.Result;
        }

        private string GetVirtualPath(string partialName)
        {
            //return $"~/2sxc/{CodeRoot.App.Site.Id}/{CodeRoot.App.Folder}/{partialName}";
            return Path.Combine(CodeRoot.App.PathSwitch(false, PathTypes.PhysRelative), partialName).ForwardSlash();
        }

        private string GetAppCodePath() =>
            Path.Combine(CodeRoot.App.PhysicalPathSwitch(false), AppCodeLoader.AppCodeFolder, AppCodeLoader.AssemblyName(CodeRoot.App.AppId)).Backslash();


        #region Connect to DynamicCodeRoot

        public void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            CodeRoot = codeRoot;
        }

        public IDynamicCodeRoot CodeRoot;

        #endregion
    }
}
