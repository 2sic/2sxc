---
uid: Specs.Data.Intro
---
# Data Models in the EAV

[!include["Before you Start"](../../shared/before-you-start-idynamicentity.md)]

## Zones (Tenants)

The EAV is multi-tenant, and each tenant is called a **Zone**. This corresponds to a _Portal_ in DNN. Each Zone contains at least 1 default app called **Content** and additional Apps as configured. 

> [!NOTE]
> The ZoneId is usually different than the DNN PortalId, 
> so [DNN has a PortalSetting for this](xref:Specs.Content.DnnIntegration). 


## Apps and Data in Apps

Every App contains [Content-Types](xref:Specs.Data.ContentTypes) and [Entities](xref:Specs.Data.Entities) - like this:  

<br>
<img src="/assets/specs/data/app-content-type-entity.png" width="100%">
<br><br>

* **Content Types** are the schema, they define what fields an entity has.  
	read more about it in [](xref:Specs.Data.ContentTypes)
* **Entities** are the data-items, they contain the content.  
	read more about it in [](xref:Specs.Data.Entities)
