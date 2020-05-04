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

Didn't have time to document this yet, sorry. If you need to know, best check the git-history.




## Version 10

### Version 10.01 - 10.09 LTS
* Develop and fine-tuning of the new Edit-UI based on Angular 8

### Version 10.20-00 to 10.20-05

* Enhanced ListCache so it will prevent parallel buildup - important for long-loading DataSources like SharePoint DataSources
* Updating to RazorBlade 3.1 which doesn't need extension methods

### Version 10.20-06

* Created `AsDynamic(string)`
* Created `AsDynamic(DataSource)` to enable `AsDynamic(Data)` instead of `AsDynamic(Data["Default"])`

### Version 10.21

* New `AsList()` for better code
* New `AsDynamic(string)` to work with json
* `/dist/` is now cleaned up on every update, to better distribute changing JS file structures

### Version 10.22

* Query Params added for Visual Query
* Created QueryRun DataSource
* Insights now includes the code file and line numbers
* Insights now also measures time needed to execute some code
* Various performance enhancements
* Improved SoC for AppsCache and AppRoot DataSource

### Version 10.23

* Lots of logging enhancements

### Version 10.24 LTS

* New stable LTS
* Improved/fixed QueryRun DataSource
* Improved Insights
* Enhancements to use 2sxc with Redis Cache
* WYSIWYG enhancements for better H1-Hx, P and Blockquote
* Performance enhancements
* Intenal refactoring for APIs
* Introduced an internal Compatibility-Level to disable very old features when using new RazorComponents

### Version 10.25 LTS

* Changed how the $2sxc client JavaScripts are loaded for much better performance and better Google PageSpeed
* Enabled various features for the Content area which previously were hidden, like Resources and Settings
* Released brand new Content-Templates App with best-practices for 10.25
* Fixed bugs with Evoq Page Publishing
* Enhanced the ValueFilter DataSource to handle dates which were null

### Version 10.26

* TinyMCE Updated to 5.1
* Enhanced `CreateInstance` API to also work when compiling Razor files from a WebApi
* New DataSource `StreamPick`
* New automatic Param called `[Params:ShowDrafts]` to be used in VisualQuery - returns `True` or `False`
* New tokens `[App:AppId]` and `[App:ZoneId]` to use in VisualQuery calles (dropdown from query)
* Changed List-Caching behaviour to create more reliable cache-keys for complex queries (previously it only went through `Default` streams to generate the cache-key)

### Version 10.27

#### Possibly breaking changes

1. Because the dynamic entity list now has a type which is dynamic, it cannot be cast to `List<dynamic>` any more. `IList<dynamic>` works, but in case you have any code casting it to `List<dynamic>` you'll need to change that to either `IList<dynamic>` or `IEnumerable<dynamic>`.

#### New Features / Major Improvements

*  Changed DynamicEntity so that accessing a property which contains many other entities it will return a `DynamicEntityWithList`. This allows Razor files to access the properties like `.EntityId` or `.FirstName` of the main entity in a sub-list easily without requiring `AsList(...)` [#1993](https://github.com/2sic/2sxc/issues/1993)
* Updated Quick-Dialog to use Angular 9, Ivy and the latest Dnn-Sxc-Angular [#1992](https://github.com/2sic/2sxc/issues/1992)
* New DataSource [](xref:ToSic.Eav.DataSources.AttributeRename) [#2004](https://github.com/2sic/2sxc/issues/2004)
* Completely refactored internal list management API [#1995]((https://github.com/2sic/2sxc/issues/1995))
* Complete refactoring of the inpage code to make it typesafe (no more `any` types)
* Created brand-new, [simpler way to create custom Toolbars](xref:HowTo.Customize.Toolbars) and [specs](xref:Specs.Cms.Toolbars.Build)
* Introduces [JS/API 2sxc-Insights](xref:HowTo.Debug.Home) for debugging In-Page code

#### Enhancements

* Performance-Enhance App DataSource to delay building objects until needed [#1991](https://github.com/2sic/2sxc/issues/1991)
* Performance-Enhance internal Token Lookup [#1998](https://github.com/2sic/2sxc/issues/1998)
* Enhanced DNN Search Index logging [#1997](https://github.com/2sic/2sxc/issues/1997)
* Corrected help-links on all data sources [#1994](https://github.com/2sic/2sxc/issues/1994)

#### Bugfixes

* Cache-All-Streams only used the Default-Streams for Cache-Key identification [#1988](https://github.com/2sic/2sxc/issues/1988)
* QueryRun DataSource doesn't show statitics on all streams [#1989](https://github.com/2sic/2sxc/issues/1989)
* Modified date and Owner information were missing on json stored entities [#2005](https://github.com/2sic/2sxc/issues/2005) / [#2006](https://github.com/2sic/2sxc/issues/2006)
* Fixed bug in JS API for non-2sxc endpoint resolution [#2000](https://github.com/2sic/2sxc/issues/2000)
* Queries didn't resolve DNN tokens when accessed in the Search Index [#1999](https://github.com/2sic/2sxc/issues/1999)



## Version 11

### Version 11.00

#### Breaking Changes

The following changes are all super-low-profile, but we want to document them just to be through:

1. `DataStream` and `IDataStream` loses a very old property called `.LightList` - we're pretty sure it's not in use anywhere, if you have it, just use `.List` instead. 
1. Old helper JS for AngularJS apps (located in `/js/angularjs`) were removed from the distribution. They had not been updated for over 3 years and we believe they were not widely used. Anybody upgrading will still preserve the files that are there. If you really need them, download an old release of 2sxc and get them manually. 

#### Enhancements

1. Razor CodeBehind
1. Automatic Polymorphism
1. Brand new Admin UI based on Angular 9 and Ivy with new Code-Editor, new Visual Query and much more
1. Updated Razor Blades to 3.02
