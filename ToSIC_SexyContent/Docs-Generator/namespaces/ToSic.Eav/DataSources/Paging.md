---
uid: ToSic.Eav.DataSources.Paging
---
# Data Source: Paging

## Purpose / Description
The **Paging** [DataSource](xref:Specs.DataSources.DataSource) is part of the [Standard EAV Data Sources](xref:Specs.DataSources.ListAll). It will limit the items returned to a _page_ containing just a few items, and will also provide a _paging-information_ so that the UI knows what page it's on and how many pages remain.  

## How to use with the Visual Query
When using the [Visual Query](xref:Temp.VisualQuery) you can just drag it into your query. This is what it usually looks like:

<img src="/assets/data-sources/paging-basic.png" width="100%">

The above example shows:

1. a content-type filter limiting the items to type _Company_
2. a Paging which only passes on the first 3 companies of page 1

## Using Url Parameters for Paging
This example shows how you can use the Url Parameter to page through the results:

<img src="/assets/data-sources/paging-page-from-url.png" width="100%">

...you could also set the page size from other tokens like url or app-configuration, like this:

<img src="/assets/data-sources/paging-page-size-app-settings.png" width="100%">

## Programming With The Paging DataSource
[!include["simpler-with-vqd"](shared-use-vqd.md)]

[!include["Read-Also-Section"](shared-read-also.md)]

[!include["Demo-App-Intro"](shared-demo-app.md)]

[!include["Heading-History"](shared-history.md)]

1. Introduced in EAV 4.x, 2sxc ?


[!include["Start-APIs"](shared-api-start.md)]