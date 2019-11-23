---
uid: Specs.DataSources.Linq
---
# Querying Data and Data Sources with code and LINQ

To get started, we recommend you read the [LINQ Guide](dotnet-query-linq-guide) and play around with the [Razor Tutorial App](https://2sxc.org/en/apps/app/razor-tutorial)

These common (extension) methods can be used on lists from 2sxc data streams (they all inherit from types implementing [IEnumerable](https://msdn.microsoft.com/de-de/library/system.collections.ienumerable(v=vs.110).aspx)):
* [.Where()](#where) - filter a list (IEnumerable) based on a specific criteria
* [.Any()](#any) - returns true if any of the elements matches a criteria
* [.OrderBy() / .OrderByDescending()](#orderby--orderbydescending) - order a list (IEnumerable) by a specific field
* [.First() / .Last()](#first--last) - get first or last element in the list
* [.Select()](#select) - transform list into a new list, selecting specific field(s)
* [.Take() / .Skip()](#take--skip) - paging functions
* [.Count()](#count) - count number of elements
* [.IndexOf()](#indexof) - find index of a specific element in the list

For a full list of all `System.Linq` methods, check out the [methods of IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable?view=netframework-4.7.2#methods)

## Using Statements

As explained in the [guide](dotnet-query-linq-guide) we recommend the following using statements in Razor:

```razor
@using System.Linq;
@using Dynlist = System.Collections.Generic.IEnumerable<dynamic>;
```

## Where
Filter a list (IEnumerable) based on a criteria.

Example: Basic filter of a list by string

```C#
var items = AsDynamic(Data["Default"]);
items = items.Where(p => p.Name == "Chuck Norris");
// items now contains only items which have "Chuck Norris" as name property
```

## Any
Returns true if any of the elements matches a criteria.

```C#
var items = AsDynamic(Data["Default"]);
var containsChuckNorris = items.Any(p => p.Name == "Chuck Norris");
// if containsChuckNorris is true, at least one element has name "Chuck Norris"
```

Here's another Any - to see if a relationship contains something. It's a bit more complex, because Razor needs to know what it's working with:

```c#
// filter - keep only those that have this Category
// note that the compare must run on the EntityId because of object wrapping/unwrapping
    postsToShow = postsToShow
        .Where(p => (p.Categories as List<dynamic>)
            .Any(c => c.EntityId == ListContent.Category[0].EntityId))

```

## OrderBy / OrderByDescending
Order a list (IEnumerable) by a specific field.

```C#
var items = AsDynamic(Data["Default"]);
items = items.OrderBy(p => p.Name);
// items are now ordered by property "Name"
```

## First / Last
Get first or last element of the list.

```C#
var items = AsDynamic(Data["Default"]);
var first = items.First(); // contains the first element
var last = items.Last(); // contains the last element
```

## Select
Transform list into a new list, selecting only specified field(s).

```C#
var items = AsDynamic(Data["Default"]);
var names = items.Select(p => p.Name); // names is a list of all names
```

## Take / Skip
Paging functions: Take only n elements, skip m elements

```C#
var items = AsDynamic(Data["Default"]);
items = items.Skip(10).Take(10); // Skips the first 10 elements and select only 10
```

## Count
Count number of elements in a list.

```C#
var items = AsDynamic(Data["Default"]);
var count = items.Count(); // contains the number of elements in the list
```

## IndexOf
Find index of a specific element in the list.

```C#
@{
    var items = AsDynamic(Data["Default"]);
}
@foreach(var item in items) {
    <h1>Item number @items.IndexOf(item)</h1>
}
```