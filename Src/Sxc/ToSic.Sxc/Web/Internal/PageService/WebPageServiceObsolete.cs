using System;
using System.Collections.Generic;
using ToSic.Lib.Coding;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Web.PageService
{
    [Obsolete]
    internal class WebPageServiceObsolete : ToSic.Sxc.Web.IPageService
    {
        public WebPageServiceObsolete(Services.IPageService pageServiceImplementation)
        {
            _pageServiceReal = pageServiceImplementation;
        }

        private Services.IPageService _pageServiceReal;

        public string SetBase(string url = null)
        {
            return _pageServiceReal.SetBase(url);
        }

        public string SetTitle(string value, string placeholder = null)
        {
            return _pageServiceReal.SetTitle(value, placeholder);
        }

        public string SetDescription(string value, string placeholder = null)
        {
            return _pageServiceReal.SetDescription(value, placeholder);
        }

        public string SetKeywords(string value, string placeholder = null)
        {
            return _pageServiceReal.SetKeywords(value, placeholder);
        }

        public string SetHttpStatus(int statusCode, string message = null)
        {
            return _pageServiceReal.SetHttpStatus(statusCode, message);
        }

        public string AddToHead(string tag)
        {
            return _pageServiceReal.AddToHead(tag);
        }

        public string AddToHead(IHtmlTag tag)
        {
            return _pageServiceReal.AddToHead(tag);
        }

        public string AddMeta(string name, string content)
        {
            return _pageServiceReal.AddMeta(name, content);
        }

        public string AddOpenGraph(string property, string content)
        {
            return _pageServiceReal.AddOpenGraph(property, content);
        }

        public string AddJsonLd(string jsonString)
        {
            return _pageServiceReal.AddJsonLd(jsonString);
        }

        public string AddJsonLd(object jsonObject)
        {
            return _pageServiceReal.AddJsonLd(jsonObject);
        }

        public string AddIcon(string path, NoParamOrder noParamOrder = default, string rel = "", int size = 0,
            string type = null)
        {
            return _pageServiceReal.AddIcon(path, noParamOrder, rel, size, type);
        }

        public string AddIconSet(string path, NoParamOrder noParamOrder = default, object favicon = null,
            IEnumerable<string> rels = null,
            IEnumerable<int> sizes = null)
        {
            return _pageServiceReal.AddIconSet(path, noParamOrder, favicon, rels, sizes);
        }

        public string Activate(params string[] keys)
        {
            return _pageServiceReal.Activate(keys);
        }
    }
}