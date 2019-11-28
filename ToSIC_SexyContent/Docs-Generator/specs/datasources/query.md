---
uid: Specs.DataSources.Queries
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

```razor

var query = App.Query["AllBlogPosts"];
var posts = query["Default"];

// you could now work with the data, or you could cast all results into dynamic objects, like...
var categories = AsDynamic(query["Categories"]);
```

Now you can loop through the data as you would otherwise, for example: 

```c#
<ol>
    @foreach(var person in AsDynamic(query["Persons"]))
    {
        <li>@person.FullName</li>
    }
</ol>
```

## Read also

* [Blog Posts about Visual Query Designer](https://2sxc.org/en/blog/tag/visual-query-designer)
* @Specs.DataSources.DataSource

## History

1. Introduced in 2sxc 07.00

