---
uid: ToSic.Eav.DataSources.EntityIdFilter
---

# Data Source: ItemIdFilter / EntityIdFilter

## Purpose / Description
The **ItemIdFilter** [DataSource](xref:Specs.DataSources.DataSource) is part of the [Standard EAV Data Sources](xref:Specs.DataSources.ListAll). It will only return the items with the id specified. 

_Warning: You should always use it in combination with a [Content-Type filter](DotNet-DataSource-ContentTypeFilter), as you want to be sure nobody can just crawl any entity you have in your system!_

## How to use with the Visual Query
When using the [Visual Query](xref:Temp.VisualQuery) you can just drag it into your query. This is what it usually looks like:

<img src="/assets/data-sources/item-id-basic.png" width="100%">

You can also use multiple values:

<img src="/assets/data-sources/item-id-multiple.png" width="100%">

Or URL-parameters:

<img src="/assets/data-sources/item-id-url.png" width="100%">


## Programming With The ItemIdFilter DataSource
[!include["simpler-with-vqd"](shared-use-vqd.md)]

[!include["Read-Also-Section"](shared-read-also.md)]

[!include["Demo-App-Intro"](shared-demo-app.md)]


[!include["Heading-History"](shared-history.md)]

1. Introduced in EAV 4.x, 2sxc ?

[!include["Start-APIs"](shared-api-start.md)]