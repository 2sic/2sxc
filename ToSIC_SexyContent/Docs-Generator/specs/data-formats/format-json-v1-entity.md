---
uid: Specs.Data.Formats.JsonV1-Entity
---

# Format: JSON Entities 

JSON based entities are items which are stored as JSON. This is used in the history, in dynamic-entities in the DB and more. 

## Format V1
### Description
1. As of now, it's using a envolope to package everything and includes a minimal header to ensure we know it's [V1](xref:Specs.Data.Formats.JsonV1). 
2. It then contains a `ContentType` node containing 
   1. various identification and description
   1. content-type metadata (array of entities) 
   1. attributes (array)
3. the attributes themselves again contain a minimal information + metadata items (entities)
4. note that the attribute order is relevant

### Example
This example is an extract of the Config Content-Type to manage the SqlDataSource (will be releasen in 2sxc 9.8 with more help-text etc.):

```json
{
    "_": {
        "V": 1
    },
    "Entity": {
        "Id": 42900,
        "Version": 6,
        "Guid": "e8a702d2-eccd-4b0f-83bd-600d8a8449d9",
        "Type": {
            "Name": "DataPipeline",
            "Id": "DataPipeline"
        },
        "Attributes": {
            "String": {
                "Description": {
                    "*": "Retrieve full list of all zones"
                },
                "Name": {
                    "*": "Zones"
                },
                "StreamsOut": {
                    "*": "ListContent,Default"
                },
                "StreamWiring": {
                    "*": "3cef3168-5fe8-4417-8ee0-c47642181a1e:Default>Out:Default"
                },
                "TestParameters": {
                    "*": "[Module:ModuleID]=6936"
                }
            },
            "Boolean": {
                "AllowEdit": {
                    "*": true
                }
            }
        },
        "Owner": "dnn:userid=1",
        "Metadata": [

        ]
    }
}
```

## Format Explained

* _ (header) mainly storing the version, in case we have to introduce a breaking change - see also [format v1](xref:Specs.Data.Formats.JsonV1)
* Entity - this marks an entity - at the moment a json package should only have 1, but later it could contain more
  * Id - the identity as a number
  * Guid - the identity as a guid
  * Type - type information
    * Name - the type name
    * Id - the type identity - usually a guid, but special types can also use a specific string
  * Attributes - the values of this entity
    * String - all the string values
      * [the field name]
        * [the languages this value applies to]
          * [the value]
        * [more languages / values]
      * [more fields / languages / values]
    * Boolean - all the boolean values
    * Number - ...
    * [more types]
  * Owner a special string identifying the owner of this item
  * Metadata (optional, array of more entities) - a list of items which further describe this entity
    * [item 1]
      * Id
      * Guid
      * [more properties]
    * [next items]


## Read also

* [Format: Json V1](xref:Specs.Data.Formats.JsonV1)
* [Blog post about the entity json format](https://2sxc.org/en/blog/post/deep-dive-json-stored-content-items-entities)

## History

1. Added in 2sxc 9.4
