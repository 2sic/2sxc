// 2024-02-27 2dm - was an experiment, will probably drop it...

//using ToSic.Eav.Plumbing;
//using ToSic.Sxc.Data;
//using ToSic.Sxc.Data.Internal.Typed;

//// ReSharper disable once CheckNamespace
//namespace Custom.Data;

//public abstract class Item16<TPresentation> : Item16
//    where TPresentation : class, ITypedItem //, ITypedItemWrapper16, new()
//{
//    public new TPresentation Presentation => _presentation
//        ??= _myItem.Presentation.NullOrGetWith(p =>
//        {
//            if (p is TPresentation pTyped) return pTyped;
//            if (_myItem is not TypedItemOfEntity typed) return null;

//            return typed.Cdf.AsCustom(typeof(TPresentation), p) as TPresentation;
//        });

//    private TPresentation _presentation;
//}