//using ToSic.Sxc.Data.Sys.Factory;

//namespace ToSic.Sxc.Data.Sys.Typed;
//internal class TypedItemOfEntityWithOverrides(IEntity entity, ICodeDataFactory cdf, bool propsRequired, IValueOverrider overrider)
//    : TypedItemOfEntity(entity, cdf, propsRequired),
//        ITyped
//{
//    string? ITyped.String(string name, NoParamOrder noParamOrder, string? fallback, bool? required, object? scrubHtml)
//        => overrider.ProcessString(name, ItemHelperForDescendants.String(name, noParamOrder, fallback, required, scrubHtml));

//    string? ITyped.Url(string name, NoParamOrder noParamOrder, string? fallback, bool? required)
//        => ItemHelperForDescendants.Url(name, noParamOrder, fallback, required, sourceOverrider: overrider);

//}