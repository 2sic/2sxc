# DataSource API: VisualQuery Attribute

## Purpose / Description
To help the Visual Query Designer properly guide the user, there is a C# _Attribute_ called **VisualQuery** to configure everything. 

## How to use VisualQuery
Here's a simple example of the [FormAndList DataSource](https://github.com/2sic/dnn-datasource-form-and-list): 
```c#
[VisualQuery(
    GlobalName = "0a0924a5-ca2f-4db5-8fc7-1a21fdbb2fbb",
    NiceName = "Dnn FormAndList",
    Type = DataSourceType.Source, 
    ExpectsDataOfType = "d98db323-7c33-4f2a-b173-ef91c0875124",
    HelpLink = "https://github.com/2sic/dnn-datasource-form-and-list/wiki")]  

```

This example shows how the the FormAndList DataSource tells the UI things like:

* the global name
* the nice name to use in the UI
* that it's a source (and not a filter) - affecting the icon shown
* that it has a content-type which should be used for the UI to configure it
* the help-link in the UI

## What you can Configure on VisualQuery
These are the things you can configure:

1. `string GlobalName` - _required_ this should be a unique id, ideally a GUID  
_important: old code use string names like a .net namespace. This should not be done any more and will be deprecated in the future_
1. `DataSourceType Type` - a type which basically controlls the icon shown in the designer. Supported values are 
    * Cache
    * Filter
    * Logic
    * Lookup
    * Modify
    * Security
    * Sort
    * Source
    * Target
1. `string ExpectsDataOfType` - if your datasource expects configuration, use this to tell the UI which content-type should be opened. Should be a GUID.  
_important: old code sometimes uses string-names. This will be deprecated in the future, so use GUIDs only_
1. `string[] In` - optional information if your source expects multiple in-sources (default is empty)
1. `string HelpLink` - a url to a page giving the user help on this DataSource
1. `string NiceName` - a nicer name to show in the UI instead of the GlobalName

Other Properties You Should Not Set:
* `string[] PreviousNames` - still work in progress and may change
* `string Icon` - still work in progress  and may change
* `bool DynamicOut` - still work in progress and may change
* `DifficultyBeta Difficulty` - still work in progress and may change

## Read also

* [DataSource API](dotnet-datasource-api) - DataSource API overview

## Demo Code and further links

* [FnL DataSource](https://github.com/2sic/dnn-datasource-form-and-list)

## History

1. Introduced in EAV 4.x, 2sxc 09.13
