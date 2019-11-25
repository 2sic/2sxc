---
uid: Specs.Data.Formats.JsonV1
---

# Format: JSON V1

JSON based data storage is used to persist data into a text (JSON) format. In 2sxc 9.4 we introduced it to store entities in the history (for version rollback). Since then it has found many new applications. 

## JSON Format V1 Package
The Json format has a minimal header like this:

```json
{
  "_": { "V": 1 }
}
```

which just contains the version. Future non-breaking enhancements will leave the version on 1 and optionally add more header information. 

In addition to that, the basic package can contain either
1. a `ContentType` node [see specs](xref:Specs.Data.Formats.JsonV1-ContentType)
1. or an `Entity` node [see specs](xref:Specs.Data.Formats.JsonV1-Entity)

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

* [Format: Json V1 Content-types](xref:Specs.Data.Formats.JsonV1-ContentType)
* [Concepts: file provided content-types](xref:Specs.Data.FileBasedContentTypes)

## History

1. Added in 2sxc 9.7

