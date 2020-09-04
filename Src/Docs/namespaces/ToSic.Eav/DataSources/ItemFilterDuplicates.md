---
uid: ToSic.Eav.DataSources.ItemFilterDuplicates
---

The **ItemFilterDuplicates** [DataSource](xref:Specs.DataSources.DataSource) is part of the [Standard EAV Data Sources](xref:Specs.DataSources.ListAll). It will provide two streams, one with all the items (but without the duplicates) and one with all the duplicates, in case you need these. 

## How to use with the Visual Query
When using the [Visual Query](xref:ToSic.Eav.DataSources.Queries.VisualQueryAttribute) you can just drag it into your query. This is what it usually looks like:

<img src="/assets/data-sources/item-filter-duplicates-basic.png" width="100%">

The above example shows two filters finding items and delivering them on the same stream. 

Because 1 item would have been delivered 2x (2sic is in Switzerland), the default-stream now only contains each item once, and the duplicates can also be retrieved from the _Duplicates_ stream if needed.

## Example Using StreamMerge
A common scenario will also combine this using the [StreamMerge](xref:ToSic.Eav.DataSources.StreamMerge). It's demonstrated there. 



## No Settings
The ItemFilterDuplicates DataSource has no settings to configure.

Note that it will automatically merge the streams in A-Z order of the In-Stream names. 

## Programming With The ItemFilterDuplicates DataSource
[!include["simpler-with-vqd"](shared-use-vqd.md)]

[!include["Read-Also-Section"](shared-read-also.md)]

[!include["Demo-App-Intro"](shared-demo-app.md)]


[!include["Heading-History"](shared-history.md)]

1. Introduced in EAV 4.x, 2sxc 9.10


[!include["Start-APIs"](shared-api-start.md)]