---
uid: Specs.Data.Entities
---
# Entities (Data, Records, Items)

Every _thing_, _record_ or _object_ in 2sxc is called an **Entity**. So if you have a list of `Book` objects, then each `Book` is an entity. 

> Many other systems use the term _Record_, _Content Item_, _Item_ or _Object_. 

[!include["Before you Start"](../../shared/before-you-start-idynamicentity.md)]

## Entity - The Data / Content Items

Entities are structured as follows:  
 
<br>
<img src="/assets/specs/data/entity.png" width="100%">
<br>

## EAV+D = Entity-Attribute-Value + Dimension

> EAV stands for **Entity**, **Attribute**, **Value**
> The D stands for **Dimension**, it says what Dimension (Language) a _Value_ is for


## How it Works

Each _Entity_ has many fields, some containing text, numbers etc. The fields an Entity can have is defined in the [Content-Type](xref:Specs.Data.ContentTypes), so each Entity is like a record of that type. 

This basic principle is used everywhere in 2sxc. For example, all these things are Entities:

* _Simple Content_ items in the _Content-App_ are entities containing a title, body and image
* [View](xref:Specs.Cms.Views) configurations are entities containing name, thumbnail, template-name etc.
* _Blog_ posts in the [Blog App](https://2sxc.org/en/apps/app/dnn-blog-app-for-dnn-dotnetnuke) are entities containing around 20 fields
* _Tag_ items in the [Blog App](https://2sxc.org/en/apps/app/dnn-blog-app-for-dnn-dotnetnuke) are also entities
* Anything you define in your apps will result in entities

#### Multilanguage Data
Each field can also be multilanguage, so there are actually many `Descriptions` in a multi-language product Entity. 

#### Relationships
Entities are much more than just records, as they can have [relationships](xref:Specs.Data.Relationships).

#### Input Forms and Fields (like WYSIWYG)
The input mask is automatically generated from the [Content-Type](xref:Specs.Data.ContentTypes). Based on the specifications, it will generate the correct [Input-Field](xref:Specs.Data.Inputs.Overview) like a simple text field, a multiline text field, a WYSIWYG or even a file-uploader. 


## APIs

* @ToSic.Eav.Data Namespace has almost everything you see here
* [](xref:ToSic.Eav.Data.IEntity) describes the main unit, the Entity
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


## Future Features & Wishes

1. Dynamic Attributes using JSON data or similar - see [](xref:Specs.Data.Values.Custom)

## History

1. Introduced in 2sxc 1.0
