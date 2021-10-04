using System;
using ToSic.SexyContent.ContentBlocks;
using ToSic.Sxc.Code;
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

        public string Render(string partialName, object model = null)
        {
            var task = _razorRendererLazy.Value.RenderToStringAsync(partialName, model ?? new {}, null);
            task.Wait();
            return task.Result;
        }

        #region Connect to DynamicCodeRoot

        public void AddBlockContext(IDynamicCodeRoot codeRoot)
        {
            CodeRoot = codeRoot;
        }

        public IDynamicCodeRoot CodeRoot;

        #endregion
    }
}
