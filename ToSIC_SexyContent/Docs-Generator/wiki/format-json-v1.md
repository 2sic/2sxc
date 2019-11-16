
# Format: JSON V1 (2sxc 9.4) 

## Purpose / Description
JSON based data storage is used to persist data into a text (JSON) format. In 2sxc 9.4 we introduced it to storte entities in the history (for version rollback). Since then it has found many new applications. 

## JSON Format V1 Package
Basically the Json format has a minimal header like
```json
{
  "_": { "V": 1 }
}
```
which just contains the version. Future non-breaking enhancements will leave the version on 1 and optionally add more header information. 

In addition to that, the basic package can contain either
1. a `ContentType` node ([see specs](Format-json-v1-content-type))
1. or an `Entity` node ([see specs](format-json-v1-entity))

This could then look like this: 

```json
{
  "_": { "V": 1 },
  "ContentType": {
    "Id": "|Config ToSic.Eav.DataSources.SqlDataSource",
    "Name": "|Config ToSic.Eav.DataSources.SqlDataSource",
    ...
  }
}
```

## Limitations
As of now (2sxc 9.7) such a package can only contain 1 root item (a content-type or an entity). Future versions may enhance this.  

## Read also
[//]: # "Additional links - often within this documentation, but can also go elsewhere"

* [Format: Json V1 Content-types](Format-json-v1-content-type)
* [Concepts: file provided content-types](Concept-File-Provided-Content-Types)

## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Added in 2sxc 9.7

[//]: # "This is a comment - for those who have never seen this"
[//]: # "The following lines are a list of links used in this page, referenced from above"
