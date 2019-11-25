---
uid: ToSic.Eav.DataSources.ValueFilter
---
# Data Source: ValueFilter

The **ValueFilter** [DataSource](xref:Specs.DataSources.DataSource) is part of the [Standard EAV Data Sources](xref:Specs.DataSources.ListAll). It will filter items based on the values - and if none are found, will optionally return a fallback-list.

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
[!include["simpler-with-vqd"](shared-use-vqd.md)]

```c#
// A source which can filter by Content-Type (EntityType)
var allAuthors = CreateSource<EntityTypeFilter>();
allAuthors.TypeName = "Author";

// filter by FullName
var someAuthors = CreateSource<ValueFilter>(allAuthors);
someAuthors.Attribute = "FullName";
someAuthors.Value = "Daniel Mettler";

```

[!include["Read-Also-Section"](shared-read-also.md)]

[!include["Demo-App-Intro"](shared-demo-app.md)]

[!include["Heading-History"](shared-history.md)]

1. Introduced in EAV 3.x, 2sxc ?
1. Enhanced in 2sxc 8.12 with fallback


[!include["Start-APIs"](shared-api-start.md)]