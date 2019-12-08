---
uid: HowTo.DynamicCode.AsDynamic
---
# AsDynamic(...) - Magic Objects

To make a complex system like the EAV work, the real objects like the [](xref:ToSic.Eav.Data.IEntity) must very very smart and complicated. This would not be fun to use in razor, where you would prefer a simple `@Something.Property` syntax. This is where `AsDynamic(...)` comes in. 

## How to Use

If you have an [](xref:ToSic.Eav.Data.IEntity) 
or you're not sure if it's either an IEntity or a [](xref:ToSic.Sxc.Data.IDynamicEntity), 
just pass it through `AsDynamic(...)` and the result will be a [](xref:ToSic.Sxc.Data.IDynamicEntity). 
You can then access the properties with the simple `thing.Property` syntax.

```razor
@inherits ToSic.Sxc.Dnn.RazorComponent
@{
    var unknown = App.Data["Default"].List.First(); // this will be an IEntity
    var myThing = AsDynamic(unknown);
}
<div>@myThing.FirstName</div>
```

> [!NOTE]
> Results of AsDynamic are dynamically typed, so you can write `.Anything` behind it. 
> But the data coming out of it is strongly typed, so `Content.Birthday` is a real date object.

> [!TIP]
> [](xref:ToSic.Sxc.Data.IDynamicEntity) objects also have some additional properties like 
> `EntityId` or `Parents(...)`. Check out the API docs.

## How it works

AsDynamic has many signatures accepting a variety of input values. It then returns an `dynamic` object which is either a [](xref:ToSic.Sxc.Data.IDynamicEntity). These are the things AsDynamic can process:

* a [](xref:ToSic.Eav.Data.IEntity) - this will return a single [](xref:ToSic.Sxc.Data.IDynamicEntity)
* [](xref:ToSic.Sxc.Data.IDynamicEntity) - will return the same [](xref:ToSic.Sxc.Data.IDynamicEntity)  
  _this option exists just so you don't have to pre-check what you pass in, making it easier to code for you_
* a `string` containing JSON - will return a dynamic object for you to program with  
  added in 2sxc 10.20

[!include["Tip Inherits"](../razor/shared-tip-inherits.md)]

## Reversing AsDynamic with AsEntity

There are rare cases where you have a [](xref:ToSic.Sxc.Data.IDynamicEntity) and want to access the real underlying [](xref:ToSic.Eav.Data.IEntity). For example if you want to really check in the code, what languages have been translated. This is very sophisticated, but you can always call `AsEntity(...)` with any [](xref:ToSic.Sxc.Data.IDynamicEntity) to get a pure [](xref:ToSic.Eav.Data.IEntity) object. 

## Obsolete use of AsDynamic() for Lists

> [!WARNING]
> In previous versions of 2sxc you'll find AsDynamic(...) also used to convert lists (IEnumerable or DataStreams) into lists. 
> This caused a lot of issues with dynamic code compilation, so in 2sxc 10.20 we introduced [AsList(...)](xref:HowTo.DynamicCode.AsList) for that use case. So if you find that kind of code, it works because...
> * without `@inherits ToSic.Sxc.Dnn.RazorComponent` in the header, the old calls still work
> * with `@inherits ToSic.Sxc.Dnn.RazorComponent` in the header, you must use `AsList(...)`

## History

1. Introduced ca. in 2sxc 1
1. Modified/added signatures in 2sxc 7
1. Added the string and DataSource signature in 2sxc 10.20
