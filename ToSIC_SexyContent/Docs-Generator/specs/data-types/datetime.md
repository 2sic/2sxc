---
uid: Specs.Data.Type.DateTime
---
# Data Type: DateTime

## Purpose / Description
DateTime data is a basic [data type](xref:Specs.Data.Type.Overview) and is used for dates and/or time values.  

## Storage in the SQL Database in the EAV-Model
This is converted to a string when stored as a string in the DB, and converted back to a .net DateTime when the data is loaded. 

## Storage in the SQL Database in the JSON-Model
This is stored as a string-value in json using the standard ISO format, as there is no official format for dates or times in JSON.

## Notes and Clarifications
Since this is a very trivial data-type, there is currently no additional documentation. 

## Read also

* [DateTime fields](xref:Specs.Data.Inputs.Boolean) documentation about using it in the UI

## History
1. Introduced in EAV 1.0 2sxc 1.0