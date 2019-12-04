---
uid: Specs.Data.Values.DateTime
---
# Data Type: DateTime

DateTime data is a basic [data type](xref:Specs.Data.Values.Overview) and is used for dates and/or time values.  

## Storage in the SQL Database in the EAV-Model
This is converted to a string when stored as a string in the DB, and converted back to a .net DateTime when the data is loaded. 

## Storage in the SQL Database in the JSON-Model
This is stored as a string-value in json using the standard ISO format, as there is no official format for dates or times in JSON.

[!include["Note-Null"](./notes-null.md)]

## Read also

* [](xref:Specs.Data.Inputs.DateTime) documentation about using it in the UI

## History
1. Introduced in EAV 1.0 2sxc 1.0