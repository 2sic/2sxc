using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using ToSic.Eav.Helpers;
using ToSic.Sxc.Code;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Razor
{
    public class RazorService : IRazorService
    {

        private readonly Lazy<IRazorRenderer> _razorRendererLazy;

        public RazorService(Lazy<IRazorRenderer> razorRendererLazy)
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

            var task = _razorRendererLazy.Value.RenderToStringAsync(razorPageVirtualPath, razorPageModel, null);
            task.Wait();
            return task.Result;
        }

        private string GetVirtualPath(string partialName)
        {
            // TODO: STV find if there is better way to provide virtual path to razorPage (cshtml).
            // This is Oqtane specific.
            return $"~/2sxc/{CodeRoot.App.Site.Id}/{CodeRoot.App.Folder}/{partialName}";
        }

        #region Connect to DynamicCodeRoot

        public void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            CodeRoot = codeRoot;
        }

        public IDynamicCodeRoot CodeRoot;

        #endregion
    }
}
