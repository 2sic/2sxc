using System;
using System.Web.WebPages;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Web
{
    public abstract partial class RazorComponentBase
    {
        [PrivateApi] protected string _ErrorWhenUsingRenderPage = null;

        public override HelperResult RenderPage(string path, params object[] data)
        {
            if (_ErrorWhenUsingRenderPage != null)
                throw new NotSupportedException(_ErrorWhenUsingRenderPage);
            return base.RenderPage(path, data);
        }

        /// <summary>
        /// This is important so that Html.Partial can still run a RenderPage, even if the public API has been disabled
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [PrivateApi]
        internal HelperResult BaseRenderPage(string path, params object[] data) 
            => base.RenderPage(path, data);

        internal Exception RenderException
        {
            get => _renderException;
            set
            {
                _renderException = value;
                if (_parentComponent != null) _parentComponent.RenderException = value;
            }
        }

        private Exception _renderException = null;

    }
}
