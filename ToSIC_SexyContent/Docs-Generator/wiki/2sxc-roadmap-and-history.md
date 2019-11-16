# 2sxc Roadmap

> ***
> We are migrating these documents to [docs.2sxc.org](https://docs.2sxc.org)
> Once it's complete, these pages will be removed
> ***


The 2sxc roadmap contains the things we think are fairly important to tackle next. Since we're all working for free, there is no commitment to do this in the order you see below. And sometimes a customer will need a feature quickly - and pay for it - then it will appear sooner. _You too can sponsor a feature!_

## Next Releases

### Version 9.?
* [ ] ability for templates to support multiple CSS frameworks at once

### Version 9.?
* [ ] RelationshipItems DataSource
* [ ] Files DataSource
* [ ] ADAM DataSource
* [ ] Pages DataSource
* [ ] Navigation DataSource

### Version 9.? - Angular5 Upgrade
* [ ] migrate quick-dialog to Angular 5
* [ ] built-in support for Angular 5


### Version 9.? - app-level JSON content-types
* [ ] Enable slow (hybrid) migration of existing (installed) system-types to new json-types
* [ ] Provide ability for Apps to also include app-level json types
* [ ] Import json content-type

### Version 9.? - In-Page List-Management
* [ ] Provide functionality to add/remove items in sub-lists (like tags) of an item, directly on the page

### Version 9.? - Anonymous Data Creation
* [ ] Ability to use WebAPIs to create data from end-users, automatically saving that data as draft
* [ ] Ability to use WebAPIs to edit data for certain user groups, automatically saving changes as draft

### Version 9.? - TypeScript Upgrade
* [ ] Refactor on-page JavaScript code to use TypeScript
* [ ] Enhance toolbar generation to provide "default-toolbar-with-minor-changes"
* [ ] Provide typings for all kinds of toolbar-generationg JS, making it easier to create custom toolbars
 
## Future
* [ ] Support for persisting data to the file-system instead of DB
* [ ] ...or whatever is needed next :)


# 2sxc History


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
* [x] [Support for DNN/Evoq Page Publishing](Concept-Dnn-Evoq-Page-Publishing)

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
* [x] Combobox input type allowing a [dropdown with values](ui-field-string-dropdown), but also allowing manual typing (to select pre-defined values, but also use Tokens)
* [x] [StreamMerge DataSource](dotnet-datasource-streammerge)
* [x] [ItemFilterDuplicates DataSource](dotnet-datasource-itemfilterduplicates)
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