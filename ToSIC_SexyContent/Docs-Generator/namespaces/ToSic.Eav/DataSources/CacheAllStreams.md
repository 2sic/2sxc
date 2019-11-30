---
uid: ToSic.Eav.DataSources.CacheAllStreams
---

# Data Source: CacheAllStreams

The **CacheAllStreams** [DataSource](xref:Specs.DataSources.DataSource) is part of the [Standard EAV Data Sources](xref:Specs.DataSources.ListAll). It will cache all streams passing through it, based on the parameters which led to the current In-state.  



## How to use with the Visual Query
When using the [Visual Query](xref:ToSic.Eav.DataSources.Query.VisualQueryAttribute) you can just drag it into your query. You must then edit the settings once to set various timeouts. Note that every in-stream exists with the same name as an out-stream. This is what it usually looks like:

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
[!include["simpler-with-vqd"](shared-use-vqd.md)]

[!include["Read-Also-Section"](shared-read-also.md)]

[!include["Demo-App-Intro"](shared-demo-app.md)]


[!include["Heading-History"](shared-history.md)]

1. Introduced in EAV 3.x, 2sxc ?


[!include["Start-APIs"](shared-api-start.md)]