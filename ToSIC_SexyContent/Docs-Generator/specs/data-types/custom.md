---
uid: Specs.Data.Values.Custom
---
# Data Type: Custom

Custom data is a basic [data type](xref:Specs.Data.Values.Overview). It's used to store JSON for special use cases. 

_Note that as of 2sxc 10, there is no API to access this yet. It's currently only used by the GPS field._

## Storage in the SQL Database in the EAV-Model
This is converted to a string when stored as a string in the DB, and converted back to a boolean when the data is loaded. 

## Storage in the SQL Database in the JSON-Model
This is simply stored as a string value in json, so kind of JSON in JSON.

## History
1. Introduced in EAV 1.0 2sxc 1.0