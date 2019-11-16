# Data Source: CacheAllStreams

## Purpose / Description
The **CacheAllStreams** [DataSource](xref:ToSic.Eav.DataSources.IDataSource) is part of the [Standard EAV Data Sources][eavds]. It will cache all streams passing through it, based on the parameters which led to the current In-state.  



## How to use with the Visual Query
When using the [Visual Query](xref:Temp.VisualQuery) you can just drag it into your query. You must then edit the settings once to set various timeouts. Note that every in-stream exists with the same name as an out-stream. This is what it usually looks like:

<img src="/assets/data-sources/cache-all-streams-basic.png" width="100%">

The above example shows:

1. a fairly complex query (from the [blog-app](https://2sxc.org/en/apps/app/dnn-blog-app-for-dnn-dotnetnuke))
2. which results in two pieces of information: the data for the current page, + the paging information
3. which is then cached

## Common Use Cases
You will typically use the **CacheAllStreams** in two scenarios: 

1. at the end of queries which have a high overhead or query so many tens-of-thousands of items, so you want to optimize
2. in the middle of queries which have sources (like CSV) which are a bit slower, thereby not re-reading that source on every request. 

## Very Smart - Automatically Considers All Parameters
Note that the cache is extremely intelligent - it will ask all in-bound sources what criterias it should use to cache, and build the cache based on that. So if some data-sources rely on an _id_ parameter in the url or a _page_ parameetr, then different URLs will result in different caches. Parameters which are not used, will not result in new caches. 



## Programming With The CacheAllStreams DataSource
_Note: We recommend to use the Visual Query where possible, as it's easier to understand and is consistant for C# and JavaScript. It's also better because it separates data-retrieval from visualization._

Because of limited resources we don't have code-examples. It works, but you'll have to figure it ouf if you need it. 

FQN: `ToSic.Eav.DataSources.Caches.CacheAllStreams`

## Read also

* [Source code of the CacheAllStreams](//github.com/2sic/eav-server/blob/master/ToSic.Eav.DataSources/Caches/CacheAllStreams.cs)
* [List of all EAV Data Sources][eavds]

## Demo App and further links
You should find some examples in this demo App
* [Demo App with examples for most DataSources](https://github.com/2sic/app-demo-visual-query/releases/latest)


## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Introduced in EAV 3.x, 2sxc ?

[//]: # "The following lines are a list of links used in this page, referenced from above"

[eavds]: DotNet-DataSources-All
