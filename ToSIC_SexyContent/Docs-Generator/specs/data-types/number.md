---
uid: Specs.Data.Values.Number
---
# Data Type: Number

Number data is a basic [data type](xref:Specs.Data.Values.Overview) and is for any kind of number 1,2,3 or very detailed numbers like 47.020503020400203 which are common in GPS coordinates. 

## Storage in the SQL Database in the EAV-Model
This is converted to a string when stored as a string in the DB, and converted back to a boolean when the data is loaded. 

## Storage in the SQL Database in the JSON-Model
This is simply stored as a number in json.

[!include["Note-Null"](./notes-null.md)]

## Read also

* @Specs.Data.Inputs.Number

## History
1. Introduced in EAV 1.0 2sxc 1.0