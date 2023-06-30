using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.AsConverter;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// Helper to convert some unknown object into the possible result.
    /// </summary>
    internal class TypedConverter
    {
        public AsConverterService AsC { get; }

        public TypedConverter(AsConverterService asC)
        {
            AsC = asC;
        }

        public (T typed, object untyped, bool ok) EvalInterface<T>(object maybe, T fallback = default) where T: class 
        {
            if (maybe == null) return (fallback, null, true);
            if (maybe is T typed) return (typed, maybe, true);
            return (null, maybe, false);
        }

        public IEntity Entity(object maybe, IEntity fallback)
        {
            var (typed, untyped, ok) = EvalInterface(maybe, fallback);
            // Try to convert, in case it's an IEntity or something; could also result in error
            return ok ? typed : AsC.AsEntity(untyped);
        }

        public ITypedItem Item(object maybe, ITypedItem fallback)
        {
            var (typed, untyped, ok) = EvalInterface(maybe, fallback);
            // Try to convert, in case it's an IEntity or something; could also result in error
            return ok ? typed : AsC.AsItem(untyped);
        }

        public IEnumerable<ITypedItem> Items(object maybe, IEnumerable<ITypedItem> fallback)
        {
            var (typed, untyped, ok) = EvalInterface(maybe, fallback);
            // Try to convert, in case it's an IEntity or something; could also result in error
            return ok ? typed : AsC.AsItems(untyped);
        }

        public IToolbarBuilder Toolbar(object maybe, IToolbarBuilder fallback)
        {
            var (typed, untyped, ok) = EvalInterface(maybe, fallback);
            // Try to convert, in case it's an IEntity or something; could also result in error
            return ok ? typed : fallback;
        }


        public IFile File(object maybe, IFile fallback)
        {
            var (typed, untyped, ok) = EvalInterface(maybe, fallback);
            if (ok) return typed;

            // Flatten list if necessary
            return untyped is IEnumerable<IFile> list ? list.First() : fallback;
        }

        public IEnumerable<IFile> Files(object maybe, IEnumerable<IFile> fallback)
        {
            var (typed, untyped, ok) = EvalInterface(maybe, fallback);
            if (ok) return typed;

            // Wrap into list if necessary
            return untyped is IFile item ? new List<IFile> { item } : fallback;
        }

        public IFolder Folder(object maybe, IFolder fallback)
        {
            var (typed, untyped, ok) = EvalInterface(maybe, fallback);
            if (ok) return typed;

            // Flatten list if necessary
            return untyped is IEnumerable<IFolder> list ? list.First() : fallback;
        }

        public IEnumerable<IFolder> Folders(object maybe, IEnumerable<IFolder> fallback)
        {
            var (typed, untyped, ok) = EvalInterface(maybe, fallback);
            if (ok) return typed;

            // Wrap into list if necessary
            return untyped is IFolder item ? new List<IFolder> { item } : fallback;
        }

        public ITypedStack Stack(object maybe, ITypedStack fallback)
        {
            var (typed, _, ok) = EvalInterface(maybe, fallback);
            return ok ? typed : null;
        }

        public ITypedRead Typed(object maybe, ITypedRead fallback)
        {
            var (typed, untyped, ok) = EvalInterface(maybe, fallback);
            // Try to convert, in case it's an IEntity or something; could also result in error
            return ok ? typed : AsC.AsItem(untyped);
        }

        #region Tags

        public IHtmlTag HtmlTag(object maybe, IHtmlTag fallback)
        {
            var (typed, _, ok) = EvalInterface(maybe, fallback);
            return ok ? typed : null;
        }
        
        public IEnumerable<IHtmlTag> HtmlTags(object maybe, IEnumerable<IHtmlTag> fallback)
        {
            var (typed, _, ok) = EvalInterface(maybe, fallback);
            return ok ? typed : null;
        }

        #endregion
    }
}
