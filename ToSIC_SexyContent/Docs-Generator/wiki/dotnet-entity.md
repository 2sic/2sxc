# IEntity / Entity Content-Item

## Purpose / Description
All content-items in the **EAV** database of 2sxc are internally handled as `Entity` objects with the interface `IEntity`. When using content-items in Razor-Templates and WebAPIs you usually don't care about this, as you will use it as a [IDynamicEntity][Dynamic Entity].

But there are some advanced cases where you need to look deeper into the object - maybe to check if a translation exists in another language, or if the value is blank because it's null, or an empty string. In this case you'll need to look at the internals, the `IEntity`.

## Difference IEntity and IDynamicEntity
Just a short piece of code, so you can see why you usually _don't want to use the IEntity_ and will prefer the _IDynamicEntity_ instead.

```cs
// the common way, using the content-item as a DynamicEntity
var titleSimple = Content.Title; 

// a bit harder, what actually happens internally
var languagePreference = ["de", "en"];
var autoResolveLinks = true;
var titleMedium = AsEntity(Content).GetBestValue("Title", languagePreference, autoResolveLinks);

// hard-core, the internals
IEntity entity = AsEntity(Content);
Dictionary<string, IAttribute> attribs = entity.Attributes;
IAttribute titleMultiLanguage = attribs["Title"];
string attType = titleMultiLanguage.Type;
IEnumerable<IValue> titleVals = titleMultiLanguage.Values;
IValue firstTitle = titleVals.First();
string firstString = firstTitle.ToString();
IEnumerable<ILanguage> langAssignments = firstTitle.Languages;
//etc.
```

As you can see, the internals provide a lot of information about the underlying data - things you usually don't care about, but in rare cases may be important.

## How to use

Note that this is a very advanced topic, and you'll need Visual Studio Intellisense to get this done reasonably. Since you'll figure it out fairly quickly, we won't document it in detail here. 

What you need to know is:

```c#
// assume that you have a DynamicEntity like Content
var entity = AsEntity(Content);

// assume that you have a DataStream with Entities...
@foreach(var postEntity in Data["Default"])
{
    var postDyn = AsDynamic(postEntity);
    // postEntity is a IEntity
    // postDyn is a DynamicEntity
}

// but this is easier - convert the whole list
@foreach(var post in AsDynamic(Data["Default"]))
{
    var postEnt = AsEntity(post);
    // do something with the @post here, it's a DynamicEntity
    // ...or with postEnt, it's an IEntity
}
```


## Read also

* To dive deeper, you must check the [EAV code][eav-core-code]


## History

1. Introduced in 2sxc 01.00
1. Multi-Language since 2sxc 02.00
1. Added `Value` and `Value<T>`, `PrimaryValue<T>` as well as `Parents()` and `Children(...)` in 09.42. Note that Value does not do the same thing as GetBestValue.

[//]: # "This is a comment - for those who have never seen this"
[//]: # "The following lines are a list of links used in this page, referenced from above"
[eav-core-code]: https://github.com/2sic/eav-server/tree/master/ToSic.Eav.Core 