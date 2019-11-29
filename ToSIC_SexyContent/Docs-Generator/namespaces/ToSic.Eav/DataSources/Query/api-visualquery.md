---
uid: ToSic.Eav.DataSources.Query.VisualQuery
---
# DataSource API: VisualQuery Attribute

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

## Read also

* [DataSource API](xref:Specs.DataSources.Api) - DataSource API overview

## Demo Code and further links

* [FnL DataSource](https://github.com/2sic/dnn-datasource-form-and-list)

## History

1. Introduced in EAV 4.x, 2sxc 09.13
