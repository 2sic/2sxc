---
uid: ToSic.Eav.DataSources.AttributeFilter
---

## Purpose / Description
The **AttributeFilter** [DataSource][ds] is part of the [Standard EAV Data Sources][eavds]. It removes values from items so that the result is smaller, and doesn't publish confidential data. It's primarily used when providing data as JSON, so that not all values are published. 

## How to use with the Visual Query
When using the [Visual Query][vqd] you can just drag it into your query. Now you can configure what properties you want and not. The following shows a demo which delivers both the data as-is, and also filtered to only deliver `Name` and `Country`:

<img src="assets/data-sources/attribute-filter-basic.png" width="100%">

This is what you get on the default-case (unfiltered):

```json
"Default": [
    {
      "Name": "2sic",
      "Country": "Switzerland",
      "Notes": "<p>Secret notes</p>",
      "Categories": [
        {
          "Id": 38646,
          "Title": "Second"
        }
      ],
      "Id": 38653,
      "Guid": "46a46d9e-f572-413c-a42e-a82ac40d929d",
      "Title": "2sic",
      "Modified": "2017-11-06T22:38:00.15Z"
    },
    ...
]
```

This is what you get on the filtered stream:

```json
"Cleaned": [
    {
      "Name": "2sic",
      "Country": "Switzerland",
      "Id": 38653,
      "Guid": "46a46d9e-f572-413c-a42e-a82ac40d929d",
      "Title": "2sic",
      "Modified": "2017-11-06T22:38:00.15Z"
    },
    ...
]
```

As you can see, the secret `Notes` and the `Categories` are not in the _Cleaned_ stream any more. 
There are three common use cases: 



## Programming With The AttributeFilter DataSource
_Note: We recommend to use the Visual Query where possible, as it's easier to understand and is consistant for C# and JavaScript. It's also better because it separates data-retrieval from visualization._

Since this data source is primarily used to slim down results in queries, the code-examples are not provided / maintained. It works, but you'll probably never need it so we don't document it. 

FQN: `ToSic.Eav.DataSources.AttributeFilter`

## Read also

* [Source code of the AttributeFilter](//github.com/2sic/eav-server/blob/master/ToSic.Eav.DataSources/AttributeFilter.cs)
* [List of all EAV Data Sources][eavds]

## Demo App and further links
You should find some examples in this demo App
* [Demo App with examples for most DataSources](https://github.com/2sic/app-demo-visual-query/releases/latest)


## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Introduced in EAV 3.x, 2sxc ?

[//]: # "The following lines are a list of links used in this page, referenced from above"
[vqd]: http://2sxc.org/en/Learn/Visual-Query-Designer
[eavds]: DotNet-DataSources-All
[ds]: DotNet-DataSource