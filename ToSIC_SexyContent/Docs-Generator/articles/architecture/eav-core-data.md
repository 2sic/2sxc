---
uid: Articles.EavCoreDataModels
---
# EAV Core Data Models

> Before you start: Remember that you usually don't need this if you are just creating Razor templates 
> or WebApis - for that, you want to use @Articles.DynamicEntity stuff.

If ever you want to know more about the internals of the core data models, here goes...

## EAV+D = Entity-Attribute-Value + Dimension

> EAV stands for **Entity**, **Attribute**, **Value**
> The D stands for **Dimension**, it says what Dimension (Language) a Value is for

<img src="/images/data-models/eav-core-data-entity.png" width="100%">

You can find out more in the API docs for

* @ToSic.Eav.Data Namespace has almost everything you see here
* @ToSic.Eav.Data.IEntity describes the main unit, the Entity
* @ToSic.Eav.Data.IContentType defines what fields exist, it's the ContentType / Schema
* @ToSic.Eav.Data.IAttributeBase, @ToSic.Eav.Data.IAttribute,  
	@ToSic.Eav.Data.IAttribute`1, @ToSic.Eav.Data.Attribute`1  
	determine the internal model how an attribute is built
* @ToSic.Eav.Data.IValue, @ToSic.Eav.Data.IValue`1, @ToSic.Eav.Data.Value`1  
	determines how values in an attribute are stored, because an attribute like `Description`  
	can have many values in different languages
* @ToSic.Eav.Data.ILanguage, @ToSic.Eav.Data.IDimension, @ToSic.Eav.Data.Language  
	languages and dimensions determine how the values are used in each language
* @ToSic.Eav.Metadata.ITarget, @ToSic.Eav.Metadata.Target  
	this determines if the Entity is by itself, or if it's enriching something else -  
	in which case this Entity is Metadata. 
* @ToSic.Eav.Metadata.MetadataOf`1  
	sometimes an Entity may itself have more metadata, which would then be stored here.

## ContentType - The Schema and Field-Definitions

<img src="/images/data-models/eav-core-data-contenttype.png" width="100%">

You can find out more in the API docs for

* @ToSic.Eav.Data Namespace has almost everything you see here
* @ToSic.Eav.Data.IContentType defines what fields exist, it's the ContentType / Schema
* @ToSic.Eav.Data.IContentTypeAttribute, @ToSic.Eav.Data.ContentTypeAttribute  
	contains the definition of an attribute
* @ToSic.Eav.Data.ContentTypeMetadata, @ToSic.Eav.Metadata.MetadataOf`1  
	contains information about the content-type (like nicer descriptions).
	This is also used for the Attribute-Metadata