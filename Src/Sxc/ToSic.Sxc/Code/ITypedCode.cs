using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Code
{
    public interface ITypedCode
    {
        dynamic Dyn { get; }
        object Get(string name, params object[] parameters);
        TValue Get<TValue>(string name, params object[] parameters);
        bool Bool(string name, params object[] parameters);
        DateTime DateTime(string name, params object[] parameters);
        string String(string name, params object[] parameters);
        int Int(string name, params object[] parameters);
        long Long(string name, params object[] parameters);
        float Float(string name, params object[] parameters);
        decimal Decimal(string name, params object[] parameters);
        double Double(string name, params object[] parameters);
        string Url(string name, params object[] parameters);
        IRawHtmlString Attribute(string name, params object[] parameters);
        ITypedItem Item(string name, params object[] parameters);
        IEnumerable<ITypedItem> Items(string name, params object[] parameters);
        IEntity Entity(string name, params object[] parameters);
        IToolbarBuilder Toolbar(string name, params object[] parameters);
        IFolder Folder(string name, params object[] parameters);
        IEnumerable<IFolder> Folders(string name, params object[] parameters);
        IFile File(string name, params object[] parameters);
        IEnumerable<IFile> Files(string name, params object[] parameters);
        ITypedStack Stack(string name, params object[] parameters);
        IHtmlTag HtmlTag(string name, params object[] parameters);
        IEnumerable<IHtmlTag> HtmlTags(string name, params object[] parameters);
        void Set<T>(string name, T value);
    }
}