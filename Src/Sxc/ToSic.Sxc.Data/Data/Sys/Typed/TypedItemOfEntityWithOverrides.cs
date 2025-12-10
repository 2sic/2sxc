//using ToSic.Sxc.Data.Sys.Factory;

//namespace ToSic.Sxc.Data.Sys.Typed;
//internal class TypedItemOfEntityWithOverrides(IEntity entity, ICodeDataFactory cdf, bool propsRequired, IValueOverrider overrider)
//    : TypedItemOfEntity(entity, cdf, propsRequired),
//        ITyped
//{
//    string? ITyped.String(string name, NoParamOrder npo, string? fallback, bool? required, object? scrubHtml)
//        => overrider.ProcessString(name, ItemHelperForDescendants.String(name, npo, fallback, required, scrubHtml));

//    string? ITyped.Url(string name, NoParamOrder npo, string? fallback, bool? required)
//        => ItemHelperForDescendants.Url(name, npo, fallback, required, sourceOverrider: overrider);

//}