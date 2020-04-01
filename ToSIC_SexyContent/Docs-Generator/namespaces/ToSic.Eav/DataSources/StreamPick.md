---
uid: ToSic.Eav.DataSources.StreamPick
---

The **StreamPick** [DataSource](xref:Specs.DataSources.DataSource) is part of the [Standard EAV Data Sources](xref:Specs.DataSources.ListAll). It will pick a stream from In and pass that on to Out.  

## How to use with the Visual Query
When using the [Visual Query](xref:ToSic.Eav.DataSources.Queries.VisualQueryAttribute) you can just drag it into your query. This is what it usually looks like:

<img src="/assets/data-sources/stream-pick-datasource-in-news-app.png" width="100%">

In this screenshot you can see:

1. The DataSource which we are configuring
1. The stream called `True`
1. The stream called `False`
1. The configuration which will resolve a token to `True` or `False` and thereby pick the stream


## Programming With The StreamPick DataSource
[!include["simpler-with-vqd"](shared-use-vqd.md)]

[!include["Read-Also-Section"](shared-read-also.md)]

[!include["Demo-App-Intro"](shared-demo-app.md)]

[!include["Heading-History"](shared-history.md)]

1. Introduced in EAV / 2sxc 10.26

[!include["Start-APIs"](shared-api-start.md)]