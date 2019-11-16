# UI Field Type: entity-query (2sxc 9.11)

## Purpose / Description
Use this field type for configuring an entity-picker storing [relationships](data-type-entity). It's an extension of the [entity field type](ui-field-entity).

The special thing about this is that the items shown for selecting are retrieved from Query and not based on a content-type name. This allows you to look up any kind of data and offer it for selection. 

## Features 
1. provide values to select from a query
1. optionally specify other streams than _Default_
1. optionally provide query parameters
1. optionally use tokens in query-parameters, to pass on values from other fields in the form
1. _todo/maybe_ data is lazy loaded, so the query is only hit when the dropdown is opened


## Configuring an Entity-Query
This shows the configuration dialog:

<img src="/assets/fields/entity/entity-query.png" width="100%">

1. **Query** the name of the query to use
1. Advanced
  1. **Parameters** a string like _country=Switzerland_ or _country=[Country]_ to parameterize the query
  1. **Stream Name** the stream name, in case you don't want the _Default_ stream

In addition to these settings, you also have the [basic entity settings](ui-field-entity) which enhance the features you are using here. 


## Important Notes
Note that this stores [item-relationships](data-type-entity) so you can only use it to select items which exist in the 2sxc/EAV database. You cannot use it to select items provided from non-entity sources, like Sql-data or File-lists. To select such data, you'll need the [String-Dropdown-Query](ui-field-string-dropdown-query) instead. 

## History
1. Introduced in EAV 4.x 2sxc 9.11
