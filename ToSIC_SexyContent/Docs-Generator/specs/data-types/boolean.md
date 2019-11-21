---
uid: Specs.Data.Type.Boolean
---
# Data Type: Boolean

## Purpose / Description
Boolean data is a basic [data type](xref:Specs.Data.Type.Overview) and is for yes/no true/false 1/0 values.  

## Storage in the SQL Database in the EAV-Model
This is converted to a string when stored as a string in the DB, and converted back to a boolean when the data is loaded. 

## Storage in the SQL Database in the JSON-Model
This is simply stored as a true/false boolean value in json.

## Notes and Clarifications
Since this is a very trivial data-type, there is currently no additional documentation. 

## Read also

* [Boolean fields](xref:Specs.Data.Inputs.Boolean) documentation about using it in the UI

## History
1. Introduced in EAV 1.0 2sxc 1.0