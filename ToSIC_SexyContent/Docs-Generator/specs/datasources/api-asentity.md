---
uid: Specs.DataSources.Api.AsEntity
---
# DataSource API: AsEntity

Many data sources generate new content items - either because they deliver some kind of information, or because they convert data from another source into standardized entities. This is where AsEntity helps. 

## How to use AsEntity
Here's a simple example of the tutorial [DateTime DataSource](https://github.com/2sic/2sxc-eav-tutorial-custom-datasource/): 

```c#
// create demo 1 entity with values related to today
// first place all values in a dictionary 
// then convert into an Entity
var today = new Dictionary<string, object>
{
    {"Title", "Date Today"},
    {"Date", DateTime.Today},
    {"DayOfWeek", DateTime.Today.DayOfWeek.ToString()},
    {"DayOfWeekNumber", DateTime.Today.DayOfWeek}
};

// ...now convert to an entity, and add to the list of results
var basic = AsEntity(today); 
var recommended = AsEntity(today, "Title", "DateTimeInfo"); 

```

This example shows how an entity-object is build using the basic syntax `AsEntity(today)` or with more configuration. 

## Concept Behind AsEntity
Internally, AsEntity will generate an `IEntity` object which is a simpler version of the normal Entity, because it's actually an `IEntityLight`, meaning it's missing some advanced features like multi-language and repository identity (which would be important in edit-scenarios). 

The simplest way is to just use `AsEntity(someDictionary)`, more advanced uses also tell the system which field is the title, some numeric or Guid IDs and more. 


## Overloads / Variations


The most common use case will be

```c#
IEntity AsEntity(Dictionary<string, object> values);
```

The full versions with all optional parameters as of 2sxc 9.13 is:


```c#
// single item
IEntity AsEntity(
    Dictionary<string, object> values,
    string titleField,
    string typeName,
    int id,
    Guid? guid,
    DateTime? modified,
    int? appId
)

// many items
IEnumerable<IEntity> AsEntity(
    IEnumerable<Dictionary<string, object>> itemValues,
    string titleField = null,
    string typeName = UnspecifiedType,
    int? appId = null
)

```
All paramaters are optional, except the values. Here's what each one does:

* `string titleField` is the title field name, so the entity then also knows which one is the title and can support `.EntityTitle` property
* `string typeName` is a nice name for the type, allowing for type-filtering later in other data sources
* `int id` gives a number identity, so `.EntityId` is usefull and filtering by EntityId (like when having details-pages needing this id) works
* `Guid guidId` is a UUID identity, so `.EntityGuid` is usefull
* `DateTime modified` would allow to filter / sort by the `.Modified` property
* `int appId` could be used to pretend it's part of another app. This only affects the AppId property, and ATM there is no important reason to do this. 


## Read also

* [DataSource API](xref:Specs.DataSources.Api) - DataSource API overview

## Demo Code and further links

* [demo data source code](https://github.com/2sic/2sxc-eav-tutorial-custom-datasource)
* [FnL DataSource](https://github.com/2sic/dnn-datasource-form-and-list)

## History

1. Introduced in EAV 4.x, 2sxc 09.13
