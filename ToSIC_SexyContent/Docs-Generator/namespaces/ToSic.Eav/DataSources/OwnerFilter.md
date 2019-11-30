---
uid: ToSic.Eav.DataSources.OwnerFilter
---
# Data Source: OwnerFilter

The **OwnerFilter** [DataSource](xref:Specs.DataSources.DataSource) is part of the [Standard EAV Data Sources](xref:Specs.DataSources.ListAll). It will only let items pass through, which a specific user (often the current one) has created initially. 

You will typically use the **OwnerFilter** in scenarios where users create their own data, and should only see/edit items which they own (usually in combination with security settings, which only allow the owner to modify their own items).

## How to use with the Visual Query
When using the [Visual Query](xref:ToSic.Eav.DataSources.Query.VisualQueryAttribute) you can just drag it into your query. You must then edit the settings once - and usually you will use the recommended prefilled-form. But you can also do something different. This is what it usually looks like:

<img src="/assets/data-sources/ownerfilter-configured.png" width="100%">

The above example shows:

1. a content-type filter limiting the items to type _Company_
2. an owner-filter which receives 5 items, but only lets 3 pass, because the _Test Settings_ have a demo-value of the user who only created 3 of the 5 items. 



## Programming With The OwnerFilter DataSource
[!include["simpler-with-vqd"](shared-use-vqd.md)]

[!include["Read-Also-Section"](shared-read-also.md)]

[!include["Demo-App-Intro"](shared-demo-app.md)]


[!include["Heading-History"](shared-history.md)]

1. Introduced in EAV 3.x, 2sxc ?


[!include["Start-APIs"](shared-api-start.md)]