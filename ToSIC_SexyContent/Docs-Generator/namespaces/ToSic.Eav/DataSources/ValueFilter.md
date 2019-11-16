# Data Source: ValueFilter

## Purpose / Description
The **ValueFilter** [DataSource](xref:ToSic.Eav.DataSources.IDataSource) is part of the [Standard EAV Data Sources][eavds]. It will filter items based on the values - and if none are found, will optionally return a fallback-list.

## How to use with the Visual Query
When using the [Visual Query](xref:Temp.VisualQuery) you can just drag it into your query. This is what it usually looks like:

<img src="/assets/data-sources/value-filter-basic.png" width="100%">

The above example shows that 2 of the 5 items fulfilled the filters requirements. 

## Using Url Parameter for Filtering
You can also filter using values from the URL, like this:

<img src="/assets/data-sources/value-filter-value-from-url.png" width="100%">

## Using Multiple URL Parameters
...and of course you can also use url parameters to specify field-names _and_ value:
<img src="/assets/data-sources/value-filter-field-value-from-url.png" width="100%">

... or field, value and operator:
<img src="/assets/data-sources/value-filter-field-value-operator-from-url.png" width="100%">

## Comparison Operators
There are many operators possible - see the in-UI help bubble for that.

## Using Fallback Streams
The filter will return the items which match the requirement, but sometimes none will match. This is common when you have a parameter from the Url, which may not match anything. In the simple version this looks like this:

<img src="/assets/data-sources/value-filter-fallback.png" width="100%">

...this previous example used a filter criteria which didn't match any items, so it resulted in delivering all. This is very useful when you want to cascade optional filters, like this:

<img src="/assets/data-sources/value-filter-chained.png" width="100%">

...this example shows two filters - the first didn't match anything (it was blank), so it delivered all items, the second one then worked, and reduced the remaining items to 2. 


## Programming With The ValueFilter DataSource
_Note: We recommend to use the Visual Query where possible, as it's easier to understand and is consistant for C# and JavaScript. It's also better because it separates data-retrieval from visualization._

Because of limited resources we don't have many code-examples: 

```c#
// A source which can filter by Content-Type (EntityType)
var allAuthors = CreateSource<EntityTypeFilter>();
allAuthors.TypeName = "Author";

// filter by FullName
var someAuthors = CreateSource<ValueFilter>(allAuthors);
someAuthors.Attribute = "FullName";
someAuthors.Value = "Daniel Mettler";

```
FQN: `ToSic.Eav.DataSources.ValueFilter`

## Read also

* [Source code of the ValueFilter](//github.com/2sic/eav-server/blob/master/ToSic.Eav.DataSources/ValueFilter.cs)
* [List of all EAV Data Sources][eavds]

## Demo App and further links
You should find some examples in this demo App
* [Demo App with examples for most DataSources](https://github.com/2sic/app-demo-visual-query/releases/latest)


## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Introduced in EAV 3.x, 2sxc ?
1. Enhanced in 2sxc 8.12 with fallback

[//]: # "The following lines are a list of links used in this page, referenced from above"

[eavds]: DotNet-DataSources-All
