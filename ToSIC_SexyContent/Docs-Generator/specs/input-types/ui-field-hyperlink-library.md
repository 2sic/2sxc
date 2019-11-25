---
uid: Specs.Data.Inputs.Hyperlink-Library
---
# Field Input-Type **hyperlink-library**

Use this field type for complete sets of files (like image galleries), storing [Hyperlink](xref:Specs.Data.Values.Hyperlink). It's an extension of the [hyperlink field type](xref:Specs.Data.Inputs.Hyperlink).

## Configuring an Hyperlink-Library

This shows the configuration dialog:

<img src="/assets/fields/hyperlink/hyperlink-library.png" width="100%">

* **Folder Depth** - if sub folders are allowed and how deply they may be nested. Use 0 for no sub folders, 1 for 1 level only, 2 for 2 levels (like /gallery/subgallery) etc. Use a large number like 100 for practically unlimited sub folders, but not recommended for realistic use cases.
* **Allow assets In Root Folder** - specifies if files may be placed in the core / root container, or if the user is required to create sub folders. This would be the case if you expect multiple groups of files, but never a top-level list. 
* **Metadata Content Types** - the content-type (or types) to be used for assets in this library. To use this, first create a content-type (like DownloadMetadata or MugshotMetadata) and type the name of the content type into this field.

## History
1.  Introduced in EAV 1.0 / 2sxc 1.0, originally as part of the [hyperlink field type](xref:Specs.Data.Inputs.Hyperlink)
2.	Changed in EAV 3.0 / 2sxc 6.0 (it used to have many configuration fields for all kinds of uses, which were then moved to sub-types)

