
---
uid: Specs.DataSources.DataStream
---

# DataStream Basics (IDataStream)

## Purpose / Description
DataStreams are objects which behave like a table, delivering a bunch of content-items. Common examples in [Razor-templates](Razor-Templates) are:

1. the [Data["Default"]](xref:HowTo.DynamicCode.Data) is a data-stream containing all content-items assigned to this template, ready to show
2. the [App.Data["Tag"]](xref:HowTo.DynamicCode.App) is a data-stream containing all tag-items in the entire app.

## How to use

The most commen uses will loop through all items in such a stream and show them. Here's an example: 

```c#
<ol>
    @foreach(var person in Data["Default"])
    {
        <li>@AsDynamic(person).FullName</li>
    }
</ol>
```
The `@foreach` will go through all the items. Each item is of the type [IEntity](DotNet-Entity). To make it easier to template, we convert it to a [Dynamic Entity](DotNet-DynamicEntity) using `AsDynamic` and then we can just show the name with `.FullName`. 

In most cases we will need the loop-item a lot, and would preferr to not write `AsDynamic` every time. Because of this, we usually write the `AsDynamic` in the Loop, like this:

```c#
<ol>
    @foreach(var person in AsDynamic(Data["Default"]))
    {
        <li>@person.FullName - born @person.Birthday and married to @person.SpouseName</li>
    }
</ol>
```

## Advanced Use Cases
There are some advanced use-cases where you need to know more about the `IDataStream` object, mostly when using LINQ. This is fairly rare, and if you really need to know more, it's best to consult the EAV DataSource code. 

Just a few more details you might care about:

1. The stream has a property `Source` which points to the owning [DataSource](xref:Specs.DataSources.DataSource). 
1. a stream might be attached to many targets for further processing or for templating, but the stream doesn't know about this
1. you can always enumerate the stream itself using LINQ, like  
    `var blues = Data["Default"].Where(x => AsDynamic(x).Category == "Blue"))` 
1. if you want more control over what you're looping through, there are two properties: 
    1. the `List` property which is a `IDictionary<int, IEntity>`, and also lets you access it directly with your entityId if known, like `Data["Default"].List[1753]`
    1. the `LightList` which is an `IEnumerable<IEntity>` 

of course there's always quite a bit more to it, like auto-caching, but you usually don't need to understand all that.  

## Read also

* todo: tutorial links
* you should also read about [DataSources](xref:Specs.DataSources.DataSource)
* todo: api links

## History

1. Introduced in 2sxc 01.00
2. Multi-Language since 2sxc 02.00

