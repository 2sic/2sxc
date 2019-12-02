---
uid: Specs.Data.ContentTypes
---
# Content-Types, aka Schemas/Object-Types

Every _thing_, _record_ or _object_ in 2sxc has a definition of fields it can have. So a `Book` may have fields like Name, Author, Title etc. This definition of the _Type_ is called a **Content-Type** and it contains specs as to the exact fields are used and what their field-types are.

> Other systems may call this _Content Type_, _Schema_, _Object-Type_ or _Table Definition_

[!include["Before you Start"](../../shared/before-you-start-idynamicentity.md)]

## ContentType - The Schema and Field-Definitions

Content-Types are structured as follows:  
 
<br>
<img src="/assets/specs/data/contenttype.png" width="100%">
<br>

## How it Works

Each [Entity](xref:Specs.Data.Entities) has many fields, some containing text, numbers etc. The fields an Entity can have is defined in the **Content-Type**, so each Entity is like a record of that type. The Content-Type will define what fields exist, what is required and what order the fields will appear in when editing the item. 

#### Storage
Most Content-Types are stored in the database, including all the Content-Types in your App. 
Special global Content-Types are stored in the file system. They are called [](xref:Specs.Data.FileBasedContentTypes)

#### Metadata of Content-Types and Attributes
Both the Content-Type and Attributes can have _Metadata_ providing more information about them. 

#### Field Types
Each field will be of a simple type like _text/string_, _number_, _boolean (yes/no)_ or other. You can find the list of [types here](xref:Specs.Data.Values.Overview).

#### Relationships
Fields can also be of type [Entity](xref:Specs.Data.Values.Entity) in which case they point to other items. This would then establish a [Relationship](xref:Specs.Data.Relationships)

#### Input Forms and Fields (like WYSIWYG)
The input mask is automatically generated from the [Content-Type](xref:Specs.Data.ContentTypes). Based on the specifications, it will generate the correct [Input-Field](xref:Specs.Data.Inputs.Overview) like a simple text field, a multiline text field, a WYSIWYG or even a file-uploader. 


## APIs

* @ToSic.Eav.Data Namespace has almost everything you see here
* @ToSic.Eav.Data.IContentType defines what fields exist, it's the ContentType / Schema
* @ToSic.Eav.Data.IContentTypeAttribute, @ToSic.Eav.Data.ContentTypeAttribute  
	contains the definition of an attribute
* @ToSic.Eav.Data.ContentTypeMetadata, @ToSic.Eav.Metadata.MetadataOf`1  
	contains information about the content-type (like nicer descriptions).
	This is also used for the Attribute-Metadata

## History

1. Introduced in 2sxc 1.0
