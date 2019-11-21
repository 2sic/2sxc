---
uid: Specs.Data.Formats.JsonV1-ContentType
---
# Format: JSON Content-Types

JSON based content-types are type-definitions which are stored as JSON. As of now we're using it to provide system-level types to the application (see [Concepts - file provided content-types](Concept-file provided-content-types)) and for various automated testing. 

## Format V1

### Description

1. As of now, it's using a envolope to package everything and includes a minimal header to ensure we know it's [V1](xref:Specs.Data.Formats.JsonV1). 
1. It then contains a `ContentType` node containing 
   1. various identification and description
   1. content-type metadata (array of entities) 
   1. attributes (array)
1. the attributes themselves again contain a minimal information + metadata items (entities)
1. note that the attribute order is relevant

### Example

This example is an extract of the Config Content-Type to manage the SqlDataSource (will be releasen in 2sxc 9.8 with more help-text etc.):

```json
{
  "_": { "V": 1 },
  "ContentType": {
    "Id": "|Config ToSic.Eav.DataSources.SqlDataSource",
    "Name": "|Config ToSic.Eav.DataSources.SqlDataSource",
    "Scope": "System",
    "Description": "todo",
    "Attributes": [
      {
        "Name": "Title",
        "Type": "String",
        "IsTitle": true,
        "Metadata": [
          {
            "Id": 0,
            "Version": 1,
            "Guid": "00000000-0000-0000-0000-000000000000",
            "Type": {
              "Name": "@All",
              "Id": "@All"
            },
            "Attributes": {
              "String": {
                "DefaultValue": { "*": "Sql Query" },
                "InputType": { "*": "string-default" }
              },
              "Boolean": { "VisibleInEditUI": { "*": true } }
            }
          }
        ]
      },
      {
        "Name": "ConnectionGroup",
        "Type": "Empty",
        "IsTitle": false,
        "Metadata": [
          {
            "Id": 0,
            "Version": 1,
            "Guid": "00000000-0000-0000-0000-000000000000",
            "Type": {
              "Name": "@All",
              "Id": "@All"
            },
            "Attributes": {
              "String": {
                "DefaultValue": { "*": "" },
                "InputType": { "*": "empty-default" }
              },
              "Boolean": { "VisibleInEditUI": { "*": true } }
            }
          }
        ]
      },
      {
        "Name": "ConnectionStringName",
        "Type": "String",
        "IsTitle": false,
        "Metadata": [
          {
            "Id": 0,
            "Version": 1,
            "Guid": "00000000-0000-0000-0000-000000000000",
            "Type": {
              "Name": "@All",
              "Id": "@All"
            },
            "Attributes": {
              "String": {
                "DefaultValue": { "*": "SiteSqlServer" },
                "InputType": { "*": "string-default" }
              },
              "Boolean": { "VisibleInEditUI": { "*": true } }
            }
          }
        ]
      },
      ...
      {
        "Name": "SelectCommand",
        "Type": "String",
        "IsTitle": false,
        "Metadata": [
          {
            "Id": 0,
            "Version": 1,
            "Guid": "00000000-0000-0000-0000-000000000000",
            "Type": {
              "Name": "@All",
              "Id": "@All"
            },
            "Attributes": {
              "String": {
                "DefaultValue": { "*": "/****** Script for SelectTopNRows command from SSMS  ******/\r\nSELECT TOP (1000) PortalId as EntityId, HomeDirectory as EntityTitle\r\n      ,[PortalID]\r\n      ,[ExpiryDate]\r\n      ,[AdministratorRoleId]\r\n      ,[GUID]\r\n      ,[DefaultLanguage]\r\n      ,[HomeDirectory]\r\n      ,[CreatedOnDate]\r\n      ,[PortalGroupID]\r\n  FROM [Portals]\r\n  Where ExpiryDate is null" },
                "InputType": { "*": "string-default" }
              },
              "Boolean": { "VisibleInEditUI": { "*": true } }
            }
          },
          {
            "Id": 0,
            "Version": 1,
            "Guid": "00000000-0000-0000-0000-000000000000",
            "Type": {
              "Name": "@string-default",
              "Id": "@string-default"
            },
            "Attributes": { "Number": { "RowCount": { "*": 10.0 } } }
          }
        ]
      }
      ...
    ],
    "Metadata": []
  }
}
```

## Details about the Format

The format is currently in version 1, and looks like this:

* _ this is the header - containing the version, in case we introduce breaking changes in the future - see also [format v1](xref:Specs.Data.Formats.JsonV1)
* ContentType - this is the content-type
  * Id - internal identifier, also known as the "static name" - often a GUID
  * Name - a nicer name, especially when the Id is a GUID
  * Scope - a term which groups types together; mainly for hiding types the user should normally not see 
  * Description - a short description for internal use
  * Attributes [array]
    * [item]
      * Name - the field-name
      * Type - the primary type, like string, number, etc.
      * Description - a short description
      * IsTitle - is this the title field (there must always be one title field)
      * Metadata [array] of [content-items](xref:Specs.Data.Formats.JsonV1-Entity) with more information about this field
* Metadata [array] of [content-items](xref:Specs.Data.Formats.JsonV1-Entity) with more information about the content-type

## Specials about the JSON Content-Types

### ID is not always a GUID

The ID is usually a GUID, but for special system types it is not. This is mostly historic, as all new content-types will have GUIDs, but old types still exist in the system which have a nice name, but that's not ideal for various use cases. 

### Scope is Like a Virtual Group

The Scope is a name - usually System or something like that. It's primarily used to group types together, so that the editor doesn't have to see the ca. 50 types in the background which make the solution work. 

### Attributes Have Metadata

Each attribute - let's say a field "Color" has more information which is needed for scenarios like the edit-UI. These items are standard [content-items](xref:Specs.Data.Formats.JsonV1-Entity) and also have the very same format as JSON entities - you can read about that in this blog.

### Content Types Have Metadata

Content-types can have a lot of metadata - also mostly for the UI. An example is the help-text which is shown. This too is stored as normal JSON [content-items](xref:Specs.Data.Formats.JsonV1-Entity).

## Limitations

As of now (2sxc 9.7) the system will pick up the content-types stored there and everything works. BUT there is no built-in mechanism to edit these. We (2sic) can easily create content-types in a normal 2sxc and export them to json for this purpose, but as of now there is no GUI to do so.

This should not affect you, as it's not meant to be managed by anybody else than us as of now. 

## Special Stuff about the JSON Format

Here are some things that may seem unusual at first:

### Content Type Attributes must preserve Sort Order

This is important, as it's relevant to the UI.

### All Attributes are Grouped by Type

Because JSON is itself a very loose data-format, and certain types like dates are not auto-detectable, we decided to have the type-specification as a first-class citizen in the format. This allows for automatic, reliable type-checking when materializing objects. 

### All values have language information

As we're usually working with real-life content-items, multi-language is always a concern. Because of this, every value is multi-language by default. If the language code is *, that means that this value is the default/fallback value for all languages. 

### Metadata is a Recursive List of Entities

2sxc and the EAV is all about real-life content-management. As such, many pieces of information have more information attached, called Metadata. Metadata-items could themselves have their own Metadata, which is then of course attached as well. 

## Read also

* [Format: Json V1](xref:Specs.Data.Formats.JsonV1)
* [Concepts: file provided content-types](Concept-File-Provided-Content-Types)
* [Blog post about json content-type definitions](https://2sxc.org/en/blog/post/deep-dive-json-content-type-definitions)


## History

1. Added in 2sxc 9.7
