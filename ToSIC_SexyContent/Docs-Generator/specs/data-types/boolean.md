---
uid: Specs.Data.Values.Boolean
---
# Data Type: Boolean

Boolean data is a basic [data type](xref:Specs.Data.Values.Overview) and is for `yes`/`no`, `true`/`false`, `1`/`0` values.  

## Storage in the SQL Database in the EAV-Model
This is converted to a string when stored as a string in the DB, and converted back to a boolean when the data is loaded. 

## Storage in the SQL Database in the JSON-Model
This is simply stored as a `true` or `false` in json.

[!include["Note-Null"](./notes-null.md)]

> A common shorthand to work with nulls is the `??` operator: `@(Content.IsAdult ?? false)`

## Read also

* [](xref:Specs.Data.Inputs.Boolean)

## History
1. Introduced in EAV 1.0 2sxc 1.0