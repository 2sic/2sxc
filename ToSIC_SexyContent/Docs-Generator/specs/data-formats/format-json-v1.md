---
uid: Specs.Data.Formats.JsonV1
---

# JSON Format V1

JSON based data storage is used to persist data into a text (JSON) format. In 2sxc 9.4 we introduced it to store entities in the history (for version rollback). Since then it has found many new applications. 

## JSON Package
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

## Special Stuff about the JSON Format

#### All values have language information

As we're usually working with real-life content-items, multi-language is always a concern. Because of this, every value is multi-language by default. If the language code is *, that means that this value is the default/fallback value for all languages. 

#### Metadata is a Recursive List of Entities

2sxc and the EAV is all about real-life content-management. As such, many pieces of information have more information attached, called Metadata. Metadata-items could themselves have their own Metadata, which is then of course attached as well. 


## Limitations
As of now (2sxc 9.7) such a package can only contain 1 root item (a content-type or an entity). Future versions may enhance this.  

## Read also

* [Format: Json V1 Content-types](xref:Specs.Data.Formats.JsonV1-ContentType)
* [Concepts: file provided content-types](xref:Specs.Data.FileBasedContentTypes)

## History

1. Added in 2sxc 9.7

