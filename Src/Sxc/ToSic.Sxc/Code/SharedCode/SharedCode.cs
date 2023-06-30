using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.AsConverter;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Code.SharedCode
{

    public class SharedCode: ServiceBase, ITypedCode
    {

        public SharedCode(object code, AsConverterService asC): base("Sxc.ShCode")
        {
            _code = code ?? throw new ArgumentNullException(nameof(code));
            _type = code.GetType();
            if (!_type.IsClass)
                throw new ArgumentException($"Object to use in {nameof(SharedCode)} must be from a class");
            _converter = new TypedConverter(asC);
        }
        private readonly object _code;
        private readonly Type _type;
        private readonly TypedConverter _converter;

        public dynamic Dyn => _code;

        public object Get(string name, params object[] parameters) => GetInternal(name, parameters);

        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;

        private object GetInternal(string name, object[] parameters)
        {
            if (name.IsEmptyOrWs()) throw new ArgumentException("Method name cannot be empty", nameof(name));

            var fieldInfo = _type.GetField(name, Flags);
            if (fieldInfo != null)
                return parameters.SafeNone()
                    ? fieldInfo.GetValue(_code)
                    : throw new ArgumentException(
                        $"Found field by the name of '{name}' but that can't accept parameters, and the code supplied {parameters.Length} parameters.",
                        nameof(parameters));

            var propInfo = _type.GetProperty(name, Flags);
            if (propInfo != null)
                return parameters.SafeNone()
                    ? propInfo.GetValue(_code)
                    : throw new ArgumentException(
                        $"Found property by the name of '{name}' but that can't accept parameters, and the code supplied {parameters.Length} parameters.",
                        nameof(parameters));

            var invoker = new CodeInvoker();
            var (ok, mResult, _) = invoker.Evaluate(_code, name, parameters ?? Array.Empty<object>());
            return ok
                ? mResult
                : throw new ArgumentException($"No field, property or method '{name} found which can use the desired parameters.");

        }

        public TValue Get<TValue>(string name, params object[] parameters) => GetV<TValue>(name, parameters);

        private T GetV<T>(string name, object[] parameters = default, [CallerMemberName] string cName = default)
        {
            var x = GetInternal(name, parameters);
            return x.ConvertOrDefault<T>();
        }

        #region Simple objects

        public bool Bool(string name, params object[] parameters) => GetV<bool>(name: name, parameters: parameters);

        public DateTime DateTime(string name, params object[] parameters) => GetV<DateTime>(name: name, parameters: parameters);

        public string String(string name, params object[] parameters) => GetV<string>(name: name, parameters: parameters);

        public int Int(string name, params object[] parameters) => GetV<int>(name: name, parameters: parameters);

        public long Long(string name, params object[] parameters) => GetV<long>(name: name, parameters: parameters);

        public float Float(string name, params object[] parameters) => GetV<float>(name: name, parameters: parameters);

        public decimal Decimal(string name, params object[] parameters) => GetV<decimal>(name: name, parameters: parameters);

        public double Double(string name, params object[] parameters) => GetV<double>(name: name, parameters: parameters);

        #endregion

        #region With processing Url / Attributes

        public string Url(string name, params object[] parameters)
        {
            var url = GetV<string>(name, parameters: parameters);
            return Tags.SafeUrl(url).ToString();
        }

        public IRawHtmlString Attribute(string name, params object[] parameters)
        {
            var value = GetV<string>(name, parameters: parameters);
            return value is null ? null : new RawHtmlString(WebUtility.HtmlEncode(value));
        }

        #endregion

        #region Complex Objects like Item, Toolbar, etc.

        public ITypedItem Item(string name, params object[] parameters) 
            => _converter.Item(GetInternal(name: name, parameters: parameters), null);

        public IEnumerable<ITypedItem> Items(string name, params object[] parameters) 
            => _converter.Items(GetInternal(name: name, parameters: parameters), null);

        public IEntity Entity(string name, params object[] parameters)
            => _converter.Entity(GetInternal(name: name, parameters: parameters), null);

        public IToolbarBuilder Toolbar(string name, params object[] parameters)
            => _converter.Toolbar(GetInternal(name: name, parameters: parameters), null);
        public IFolder Folder(string name, params object[] parameters)
            => _converter.Folder(GetInternal(name: name, parameters: parameters), null);
        public IEnumerable<IFolder> Folders(string name, params object[] parameters)
            => _converter.Folders(GetInternal(name: name, parameters: parameters), null);

        public IFile File(string name, params object[] parameters)
            => _converter.File(GetInternal(name: name, parameters: parameters), null);
        public IEnumerable<IFile> Files(string name, params object[] parameters) 
            => _converter.Files(GetInternal(name: name, parameters: parameters), null);

        public ITypedStack Stack(string name, params object[] parameters) 
            => _converter.Stack(GetInternal(name: name, parameters: parameters), null);

        public IHtmlTag HtmlTag(string name, params object[] parameters) 
            => _converter.HtmlTag(GetInternal(name: name, parameters: parameters), null);

        public IEnumerable<IHtmlTag> HtmlTags(string name, params object[] parameters) 
            => _converter.HtmlTags(GetInternal(name: name, parameters: parameters), null);

        #endregion


        #region Set

        public void Set<T>(string name, T value)
        {
            try
            {
                var fieldInfo = _type.GetField(name, Flags);
                if (fieldInfo != null)
                {
                    fieldInfo.SetValue(_code, value);
                    return;
                }

                var propInfo = _type.GetProperty(name, Flags);
                if (propInfo != null)
                {
                    propInfo.SetValue(_code, value);
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Problem with {nameof(name)} = '{name}' and {nameof(value)} = '{value}'", ex);
            }

            throw new ArgumentException($"Can't find field or property by the name '{name}'");
        }

        #endregion
    }
}
