---
uid: Specs.Roadmap.History
---

# History of the EAV and 2sxc Code Base

Here we'll track important changes, especially feature additions. [](xref:Specs.Roadmap.BreakingChanges) may also be relevant to you. 



## Version 1-7 (2012-2016)
Didn't find time to document this :)

## Version 8
### Version 8.00 - 8.8
Didn't find time to document this :)

### Version 8.09
* [x] Very Rich Content (Inner Content 2.0)
* [x] Item-Delete directly from in-page toolbar

### Version 8.10
* [x] Shuffle data sources (to randomize items)
* [x] Public (anonyomous) REST API for query and read/write content-items

### Version 8.11-8.12
Mostly smaller bugfixes


## Version 9

### Version 9.0
* [x] Change data access to Entity Framework Core 1.1
* [x] Change IoC Layer to use .net Core mechanisms 
* [x] Replace Quick-Dialogs with Angular4 implementation

### Version 9.1
* [x] Move primary quick-dialog GUI to bottom of page

### Version 9.3
* [x] Item-Level versioning, history and rollback

### Version 9.4
* [x] Drop all dependencies to Telerik - file browser using ADAM

### Version 9.5
* [x] [Support for DNN/Evoq Page Publishing](xref:Specs.Cms.PagePublishing)

### Version 9.6
* [x] [Extensive logging system to watch all internals](https://2sxc.org/en/blog/post/releasing-2sxc-9-6-with-extensive-logging)

### Version 9.7 - the JSON-Content-Types & Entities Upgrade
* [x] New features in entity json serialization
  * [x] Support for schema-free (very dynamic) entities
* [x] new features in content-type json serialization
    * [x] defined json format for content-types
    * [x] full serialization and deserialization of json-based content types
* [x] SQL IRepository storage enhancements
    * [x] extended SQL table Entities to also store AppId and ContentType (name) to ensure that json-entities can be stored
    * [x] Support to persist entities as JSON in repository (DB)
* [x] file-storage implementation of IRepository loader, to created a standard-based app-content-types provider
    * [x] Ability to provide file-based json content-types at a system level, which is probably the better solution for most scenarios (more flexible, easier to spot changes, etc.) 
* [x] global content-types system
    * [x] Support for code-provided content-types, which allows faster feature-evolution
    * [x] ~~Support for JSON based i18n on code-provided content-types, to allow better translation~~ removed again, as not needed
* [x] Ensure export/import of data of these new content-types (req. extensive refactoring)
* [x] extensive automated testing of these new features


### Version 9.8 - the Visual Query Upgrade
* [x] SqlDataSource in Visual Query Designer
* [x] Show DataSources which have Fallback-In-Streams in Visual Query Designer
* [x] UI Updates on Visual Query Designer, to better fit current needs
* [x] More help documentation for various data sources in Visual Query Designer
* [x] Shuffle DataSource now configurable in visual query
* [x] Support for Schema-Free Content (dynamic, without existing content-type)

### Version 9.9 - another Visual Query Upgrade
* [x] Enhance relationship filter to enable filtering on other fields of related items
* [x] Enhance other data sources with features which so far were not available in the visual designer

### Version 9.10 - Combobox and more DataSources (WIP)
* [x] Combobox input type allowing a [dropdown with values](xref:Specs.Data.Inputs.String-Dropdown), but also allowing manual typing (to select pre-defined values, but also use Tokens)
* [x] [StreamMerge DataSource](xref:ToSic.Eav.DataSources.StreamMerge)
* [x] [ItemFilterDuplicates DataSource](xref:ToSic.Eav.DataSources.ItemFilterDuplicates)
* [x] feature to export Json ContentTypes
* [x] multiple file-repos which deliver Content-Types, allows for any module to provide additional contenttypes

### Version 9.11 - Query-Picker & more DataSources
* [x] Entity-Picker delivering items from a query, instead of a type
* [x] string-dropdown-query to pick string-items from a query instead of pre-filled
* [x] query export / import
* [x] multi-select items in a string-query-picker

### Version 9.12
* [x] Json-based global query definitions
* [x] Method to add parameters to a called query (like when using an entity-pickers which uses a query)
* [x] pre-build queries for things like zones, apps, content-types, fields, query-info etc.
* [x] data sources for Zones, Apps, Queries, Attributes, etc.
* [x] limit streams returned by a query

### Version 9.13
* [x] Enhanced API to create custom DataSources + ca. 10 blog posts for that
* [x] Standalone FnL / UDT DataSource (removed it from core distribution)

### Version 9.14 LTS
* [x] New LTS Concept - see [blog post about LTS 9.14](https://2sxc.org/en/blog/post/special-edition-2sxc-9-14-lts-long-term-support)


### Version 9.15-9.42 LTS
didn't have time to document this yet, sorry.




## Version 10

### Version 10.01 - 10.09 LTS
* Develop and fine-tuning of the new Edit-UI based on Angular 8

### Version 10.20.05

* Enhanced ListCache so it will prevent parallel buildup - important for long-loading DataSources like SharePoint DataSources
* Updating to RazorBlade 3.1 which doesn't need extension methods