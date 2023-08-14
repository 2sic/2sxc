using System;
using System.Net;
using System.Runtime.CompilerServices;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Data.Typed.TypedHelpers;

namespace ToSic.Sxc.Data
{
    internal class CodeItemHelper
    {
        public readonly GetAndConvertHelper Helper;

        public CodeItemHelper(GetAndConvertHelper helper)
        {
            Helper = helper;
        }

        #region Get

        public object Get(string name, string noParamOrder, bool? required, [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, nameof(required), methodName: cName);
            var findResult = Helper.TryGet(name);
            return IsErrStrict(findResult.Found, required, Helper.StrictGet)
                ? throw ErrStrict(name, cName)
                : findResult.Result;
        }

        public TValue G4T<TValue>(string name, string noParamOrder, TValue fallback, bool? required = default, [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, nameof(fallback), methodName: cName);
            var findResult = Helper.TryGet(name);
            return IsErrStrict(findResult.Found, required, Helper.StrictGet)
                ? throw ErrStrict(name, cName)
                : findResult.Result.ConvertOrFallback(fallback);
        }


        #endregion

        public IRawHtmlString Attribute(string name, string noParamOrder, string fallback, bool? required)
        {
            var result = Get(name, noParamOrder, required);
            var strValue = Helper.Cdf.Services.ForCode.ForCode(result, fallback: fallback);
            return strValue is null ? null : new RawHtmlString(WebUtility.HtmlEncode(strValue));
        }

        public string String(string name, string noParamOrder, string fallback, bool? required, bool scrubHtml)
        {
            var value = G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);
            return scrubHtml ? Helper.Cdf.Services.Scrub.All(value) : value;
        }

        public string Url(string name, string noParamOrder, string fallback, bool? required)
        {
            // TODO: STRICT
            var url = Helper.GetInternal(name, lookupLink: true).Result as string;
            return Tags.SafeUrl(url).ToString();
        }



        public IField Field(ITypedItem parent, string name, string noParamOrder = Protector, bool? required = default)
        {
            Protect(noParamOrder, nameof(required));
            // TODO: make sure that if we use a path, the field is from the correct parent
            if (name.Contains(PropertyStack.PathSeparator.ToString()))
                throw new NotImplementedException("Path support on this method is not yet supported. Ask iJungleboy");

            return IsErrStrict(parent, name, required, Helper.StrictGet)
                ? throw ErrStrict(name)
                : new Field(parent, name, Helper.Cdf);
        }

    }
}
