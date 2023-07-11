using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ToSic.Eav.Data;
using ToSic.Eav.Generics;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using ToSic.Sxc.Edit.Toolbar;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Code
{
    public class TypedModel : ITypedModel
    {
        private readonly bool _isRazor;
        private readonly string _razorFileName;
        private readonly IDictionary<string, object> _paramsDictionary;
        private readonly TypedConverter _converter;

        public TypedModel(IDictionary<string, object> paramsDictionary, IDynamicCodeRoot codeRoot, bool isRazor, string razorFileName)
        {
            _isRazor = isRazor;
            _razorFileName = razorFileName;
            _paramsDictionary = paramsDictionary?.ToInvariant() ?? new Dictionary<string, object>();
            _converter = new TypedConverter(codeRoot.AsC);
        }

        #region Check if parameters were supplied

        public bool HasAll(params string[] names)
        {
            if (names == null || names.Length == 0) return true;
            return names.All(n => _paramsDictionary.ContainsKey(n));
        }

        public bool HasAny(params string[] names)
        {
            if (names == null || names.Length == 0) return true;
            return names.Any(n => _paramsDictionary.ContainsKey(n));
        }

        public string RequireAny(params string[] names)
        {
            if (HasAny(names)) return null;
            throw new ArgumentException(RequireMsg("one or more", "none", names));
        }
        public string RequireAll(params string[] names)
        {
            if (HasAll(names)) return null;
            throw new ArgumentException(RequireMsg("all", "not all", names));
        }

        private string RequireMsg(string requires, string but, string[] names) =>
            $"Partial Razor '{_razorFileName}' requires {requires} of the following parameters, but {but} were provided: " +
            string.Join(", ", (names ?? Array.Empty<string>()).Select(s => $"'{s}'"));

        #endregion

        #region Get and GetInternal

        public object Get(string name, string noParamOrder = Protector, bool? required = default) 
            => GetInternal(name, required, noParamOrder);

        public T Get<T>(string name, string noParamOrder = Protector, T fallback = default, bool? required = default) 
            => GetInternal(name, noParamOrder, fallback, required);

        private T GetInternal<T>(string name, string noParamOrder, T fallback, bool? required, [CallerMemberName] string method = default)
        {
            // If we have a clear fallback, don't make it required
            if (fallback != null && !EqualityComparer<T>.Default.Equals(fallback, default)) required = false;

            var found = GetInternal(name, required, noParamOrder, method: method);
            
            if (found == null) return fallback;

            // Already matching type OR Interface (because ConvertOrFallback can't handle interface)
            if (found is T typed) return typed;

            return typeof(T).IsInterface ? fallback : found.ConvertOrFallback(fallback);
        }

        private object GetInternal(string name, bool? required, string noParamOrder = Protector, [CallerMemberName] string method = default)
        {
            Protect(noParamOrder, "required, fallback", method);

            if (_paramsDictionary.TryGetValue(name, out var result))
                return result;
            if (required == false)
                return null;

            var call = $"{nameof(TypedModel)}.{method}(\"{name}\")";
            var callReqFalse = call.Replace(")", ", required: false)");
            throw new ArgumentException($@"Tried to get parameter with {call} but parameter '{name}' not provided. 
Either change the calling Html.Partial(""{_razorFileName}"", {{ {name} = ... }} ) or use {callReqFalse} to make it optional.", nameof(name));
        }

        #endregion

        public dynamic Dynamic(string name, string noParamOrder = Protector, object fallback = default, bool? required = default) 
            => GetInternal(name, noParamOrder, fallback, required);

        #region Numbers

        public int Int(string name, string noParamOrder = Protector, int fallback = default, bool? required = default) 
            => GetInternal(name, noParamOrder, fallback, required);

        public float Float(string name, string noParamOrder = Protector, float fallback = default, bool? required = default) 
            => GetInternal(name, noParamOrder, fallback, required);

        public double Double(string name, string noParamOrder = Protector, double fallback = default, bool? required = default) 
            => GetInternal(name, noParamOrder, fallback, required);

        public decimal Decimal(string name, string noParamOrder = Protector, decimal fallback = default, bool? required = default) 
            => GetInternal(name, noParamOrder, fallback, required);

        #endregion

        #region Standard value types

        public string String(string name, string noParamOrder = Protector, string fallback = default, bool? required = default) 
            => GetInternal(name, noParamOrder, fallback, required);

        public Guid Guid(string name, string noParamOrder = Protector, Guid fallback = default, bool? required = default) 
            => GetInternal(name, noParamOrder, fallback, required);

        public bool Bool(string name, string noParamOrder = Protector, bool fallback = default, bool? required = default) 
            => GetInternal(name, noParamOrder, fallback, required);

        public DateTime DateTime(string name, string noParamOrder = Protector, DateTime fallback = default, bool? required = default) 
            => GetInternal(name, noParamOrder, fallback, required);

        #endregion


        //#region Stacks

        //public ITypedStack Stack(string name, string noParamOrder = Protector, ITypedStack fallback = default, bool? required = default) 
        //    => _converter.Stack(GetInternal(name, required, noParamOrder), fallback);

        //#endregion

        #region Adam

        public IFile File(string name, string noParamOrder = Protector, IFile fallback = default, bool? required = default) 
            => _converter.File(GetInternal(name, required, noParamOrder), fallback);

        public IEnumerable<IFile> Files(string name, string noParamOrder = Protector, IEnumerable<IFile> fallback = default, bool? required = default) 
            => _converter.Files(GetInternal(name, required, noParamOrder), fallback);

        public IFolder Folder(string name, string noParamOrder = Protector, IFolder fallback = default, bool? required = default)
            => _converter.Folder(GetInternal(name, required, noParamOrder), fallback);

        public IEnumerable<IFolder> Folders(string name, string noParamOrder = Protector, IEnumerable<IFolder> fallback = default, bool? required = default)
            => _converter.Folders(GetInternal(name, required, noParamOrder), fallback);

        #endregion

        #region Entity and Item(s)

        public ITypedItem Item(string name, string noParamOrder = Protector, ITypedItem fallback = default, bool? required = default)
            => _converter.Item(GetInternal(name, required, noParamOrder), noParamOrder, fallback);

        public IEnumerable<ITypedItem> Items(string name, string noParamOrder = Protector, IEnumerable<ITypedItem> fallback = default, bool? required = default)
            => _converter.Items(GetInternal(name, required, noParamOrder), noParamOrder, fallback);

        #endregion

        #region HtmlTags

        public IHtmlTag HtmlTag(string name, string noParamOrder = Protector, IHtmlTag fallback = default, bool? required = default)
            => _converter.HtmlTag(GetInternal(name, required, noParamOrder), fallback);

        public IEnumerable<IHtmlTag> HtmlTags(string name, string noParamOrder = Protector, IEnumerable<IHtmlTag> fallback = default, bool? required = default)
            => _converter.HtmlTags(GetInternal(name, required, noParamOrder), fallback);


        #endregion

        #region Toolbar

        public IToolbarBuilder Toolbar(string name, string noParamOrder = Protector, IToolbarBuilder fallback = default, bool? required = default)
            => _converter.Toolbar(GetInternal(name, required, noParamOrder), fallback);

        #endregion
    }
}
