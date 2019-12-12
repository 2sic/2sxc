---
uid: Specs.DataSources.Query
---

# Query Basics

Queries are special Data-Sources which internally chain various other Data-Sources to query the underlying data. They can be built in two ways:

1. Directly in your code
1. Built from a configuration which is itself stored as a set of entities

Such queries are then used to give data to a view, to a JavaScript SPA (throug a simple REST API) or to your code, which can then perform other operations with the data. 

## How to use

1. Usually you'll configure queries in the VisualQuery Designer in the App settings. Here's a [Getting Started with Visual Query](https://2sxc.org/en/learn/visual-query-designer). 
1. There are a lot of [Blogs about Visual Query Designer](https://2sxc.org/en/blog/tag/visual-query-designer) that will help you start.

Then you will usually configure a view to use this.

## Using Queries in your Code

In a Razor or WebApi file, you can always write something like this

```c#
var query = App.Query["AllBlogPosts"];
var posts = query["Default"];

// you could now work with the data, or you could cast all results into dynamic objects, like...
var categories = AsList(query["Categories"]);

// if all you need is the "Default" stream as dynamic, you can write
var posts = AsList(query);
```

Now you can loop through the data as you would otherwise, for example: 

```c#
<ol>
    @foreach(var person in AsList(query["Persons"]))
    {
        <li>@person.FullName</li>
    }
</ol>
```

## Technical Implementation

The data which defines a query is stored as IEntity data. 

* So there is a header IEntity which is read through an [](xref:ToSic.Eav.DataSources.Queries.QueryDefinition).
* It contains the name, and a bunch of metadata IEntity items which are read as [](xref:ToSic.Eav.DataSources.Queries.QueryPartDefinition). 
* It also contains a list of [Connections](xref:ToSic.Eav.DataSources.Queries.Connection) which define how the DataSources pass data from one source to another.
* There are also test-parameters on such a query, which are only used for testing in the [VisualQuery Designer](xref:Specs.DataSources.VisualQuery)

## Read also

* [](xref:Specs.DataSources.QueryParams)
* [Blog Posts about Visual Query Designer](https://2sxc.org/en/blog/tag/visual-query-designer)
* [](xref:Specs.DataSources.DataSource)


## History

1. Introduced in 2sxc 07.00

