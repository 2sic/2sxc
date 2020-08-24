---
uid: Specs.Data.Inputs.String-Dropdown-Query
---
# Field Input-Type **string-dropdown-query**

Use this field type for configuring a dropdown UI elements, storing [string/text data](xref:Specs.Data.Values.String). It's an extension of the [string field type](xref:Specs.Data.Inputs.String).

The special thing about this is that the items shown for selecting are retrieved from Query and not pre-defined as part of the the field definition. This allows you to look up any kind of data and offer it for selection. 

## Features 
1. provide values to select from a query
1. optionally provide query parameters
1. optionally use tokens in query-parameters, to pass on values from other fields in the form
1. provide visible labels which are different from the stored value
1. you can configure which field is stored (like an ID etc.)
1. you can configure which field is shown visible in the drop-down
1. data is semi-lazy loaded, so the query is only hit again, if the dropdown is opened after parameters change
2. optionally allow users to type in something different, in scenarios where this is important

## Configuring a String-Dropdown-Query
This shows the configuration dialog:

<img src="/assets/fields/string/string-dropdown-query.png" width="100%">

1. **Query** the name of the query to use
1. Advanced
  1. **Parameters** a string like _country=Switzerland_ or _country=[Country]_ to parameterize the query
  1. **Stream Name** the stream name, in case you don't want the _Default_ stream
  1. **Value Field** the value which is stored - basically the field you want in your string at the end
  1. **Label Field** the label which is shown - basically for nice display in the UI
1. Multi-Select
  1. **Multiple Items** enable this if you want to allow multi-select
  1. **Separator** the character which will separate the selected items, like "company1,company2,company3"

## Special Behaviour
1. When the drop-down UI element finds data stored, which doesn't match any of the values it has available, it will leave that data intact unless the users selects something manually

## History
1. Introduced in EAV 4.5 2sxc 9.11
1. Enhanced with options to allow edit/delete in 10.20