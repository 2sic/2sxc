---
uid: ToSic.Eav.DataSources.StreamFallback
---

The **StreamFallback** [DataSource](xref:Specs.DataSources.DataSource) is part of the [Standard EAV Data Sources](xref:Specs.DataSources.ListAll). It will pass on the first stream which has results. The order of evaluation is based on the names of the streams as they enter the StreamFallback. 

## How to use with the Visual Query
When using the [Visual Query](xref:ToSic.Eav.DataSources.Query.VisualQueryAttribute) you can just drag it into your query. This is what it usually looks like:

<img src="/assets/data-sources/stream-fallback-4-stream-example.png" width="100%">

The above example shows a query from the [blog app](https://2sxc.org/en/apps/app/dnn-blog-app-for-dnn-dotnetnuke):

1. a stream-fallback with 4 in-streams
2. the first stream which would filter by tag, if the URL had a tag in it - as the test-data has no tag, this stream delivers no results
2. the second stream filters by author - as the url would have an author (specified in the test-values), it does return items

The StreamFallback therefor passes on the items provided by the author-filter. Note that while using the visual query, all other streams are also processed to show what's happening. At runtime it will stop processing other streams once it's found a first match. 


## Programming With The StreamFallback DataSource
[!include["simpler-with-vqd"](shared-use-vqd.md)]

[!include["Read-Also-Section"](shared-read-also.md)]

[!include["Demo-App-Intro"](shared-demo-app.md)]

[!include["Heading-History"](shared-history.md)]

1. Introduced in EAV 3.x, 2sxc ?

[!include["Start-APIs"](shared-api-start.md)]