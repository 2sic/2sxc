---
uid: Specs.DataSources.QueryParams
---

# Query Parameters / Tokens

Queries can be parameterized with token placeholders like `[QueryString:Id]`, `[App:Settings:PageSize]` or `[Params:Sort]` _(new in v10.22)_. 

To understand these placeholder tokens, best read up on [](xref:Specs.DataSources.Configuration) and [](xref:Specs.LookUp.Tokens). 
There you'll read about how tokens work, where they come from, how to use default/fallback values and more. 

This page explains how Queries have four special types of parameters, namely these:

1. Special LookUp: `[Params:...]` (new in v10.22)
1. Special LookUp: `[In:...]`
1. Option to override the values during testing with `TestParameters`

## `Params` LookUp (new in 10.22)

The query now has a special LookUp for the source `Params`. This should help you make nicer, more flexible queries and also allow you to set these query parameters from C# code. 

> [!TIP]
> You can set these params in the Query configuration dialog
> TODO: PIC

> [!NOTE]
> Your queries can also use params that you didn't configure - but they will return nothing unless you would set them in your C# code before you run the query.

> [!TIP]
> We recommend to always use the Params for everything as it's simpler to see all the parameters your query depends on.  
> So if you have a [ValueFilter](xref:ToSic.Eav.DataSources.ValueFilter) expecting a value from the url, we recommend that you  
> Create a param `FilterLastName=[QueryString:LastName]`  
> In your data-source configuration, use `[Params:FilterLastName]`

## `In` LookUp

Data Sources in a query can also retrieve values from another data-source which is a source. Use the `[In:source-name:attribute-name]` syntax. 
So if you have a Value DataSource which has the `Default`-in with the data to filter, and a `Module`-in comes from the Module Settings, 
you would write `[In:Module:Category]` to filter by the the category as selected in the module settings. 

## Testing your Query with Test Parameters 

The [VisualQuery Designer](xref:Specs.DataSources.VisualQuery) also allows you to set test-values for testing the query. The test-values should define all the full tokens to replace. Example:

```
[QueryString:Id]=27
[QueryString:SortOrder]=Desc
[Params:FilterLastName]=Mettler
[Params:FilterNameSort]=[QueryString:SortOrder||Asc]
```

> [!TIP]
> As you can see in the example, even test params can contain more tokens if you need them. 
> In the above example, `Params:FilterNameSort` would resolve to `Desc` 
> because it will first check the `QueryString:SortOrder` which also has a test-value of `Desc`.

## Setting Query Parameters in your Code

In a Razor or WebApi file, you can always write something like this

```c#
var allPosts = App.Query["AllBlogPosts"];
allPosts.Params("Category", "Web");
var posts = allPosts["Default"];

var dynPosts = AsList(posts);
```

> [!WARNING]
> Query objects are single use - which is an internal optimization for reliable, rapid access. 
> So if you retrieve various streams, the query still only executes once.  
> But if you set a parameter after running the query, you will get an error, unless you call `Reset()` first. 
> See the next example:

```c#
var query = App.Query["AllBlogPosts"];
query.Params("Category", "Web");
var webPosts = AsList(query);

// this would result in an error
// allPosts.Params("Category", "IT");

// this works
query.Reset();
query.Params("Category", "IT");
var itPosts = AsList(query);
```



## Read also

* [VisualQuery Designer](xref:Specs.DataSources.VisualQuery)
* APIs
    * [](xref:ToSic.Eav.DataSources.Queries.Query)
    * [](xref:ToSic.Eav.DataSources.Queries.QueryDefinition)
    * [](xref:ToSic.Eav.DataSources.Queries.QueryPartDefinition)
* [Blog Posts about Visual Query Designer](https://2sxc.org/en/blog/tag/visual-query-designer)

## History

1. Introduced in 2sxc 07.00
1. In added in 2sxc 07.00
1. Params added in 2sxc 10.22

