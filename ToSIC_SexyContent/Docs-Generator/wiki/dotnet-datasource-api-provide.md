# DataSource API: Provide

## Purpose / Description
DataSources always provide data on an `Out` stream. The `Provide` method makes it very easy to do. 

## How to use Provide
In general, you need to 
1. have a private method - usually called `GetList()` that would return a _list_ of items, if it is called
1. use that `GetList()` to build a stream, which has a name - typically _Default_
1. attach that stream to the `Out` so that it's accessible

Here's a simple example of the constructor of the [DnnFormAndList DataSource](https://github.com/2sic/dnn-datasource-form-and-list), which provides the default stream: 

```c#
public DnnFormAndList()
{
    // Specify what out-streams this data-source provides. Usually just one, called "Default"
    Provide(GetList); // does everything

    // ...
}
private IEnumerable<Eav.Interfaces.IEntity> GetList() 
{
    // ...
} 
```
This example ensures that the `.Out["Default"]` as well as the `.List` (which is a shorthand for `.Out[Constants.DefaultStreamName].List`) are mounted, ready to deliver.

## Overloads

* `Provide(listfunction)` - default version, which provides the "Default" stream
* `Provide(name, listfunction)` - alternative for named streams when your DataSource has more streams. 

## Providing multiple streams
In case you want to offer multiple streams (like one containing products, the other categories), the common pattern is:

```c#
public SomeConstructor()
{
    Provide(GetProducts);
    Provide("Categories", GetCategories);

    // ...
}
```



## Read also

* [DataSource API](dotnet-datasource-api) - DataSource API overview
* [Ensuring configuration is parsed](dotnet-datasource-api-ensureconfigurationisloaded)

## Demo App and further links

* todo

## History

1. Introduced in EAV 4.x, 2sxc 09.13
