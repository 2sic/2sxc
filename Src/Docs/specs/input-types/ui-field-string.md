---
uid: Specs.Data.Inputs.String
---
# Field Input-Type **string**

The **string** field is the base type for input-fields storing [string/text data](xref:Specs.Data.Values.String). It offers many sub-types.

## Features 
The basic **string** field doesn't have any features, since all the features are in the sub-types. 

## Sub-Types of String Fields

1. [string-default](xref:Specs.Data.Inputs.String-Default) - simple one or multi-line inputs
1. [drop-down](xref:Specs.Data.Inputs.String-Dropdown) for simple dropdowns
1. [drop-down-query](xref:Specs.Data.Inputs.String-Dropdown-Query) for dropdowns which retrieve the data from a server
1. [wysiwyg](xref:Specs.Data.Inputs.String-Wysiwyg)
1. [font-icon-picker](xref:Specs.Data.Inputs.String-Font-Icon-Picker)
1. [url-path](xref:Specs.Data.Inputs.String-Url-Path)

## Shared Settings
All string field types currently don't have shared settings. 

## History

1. Introduced in EAV 1.0 / 2sxc 1.0
2. Changed in EAV 3.0 / 2sxc 6.0 (it used to have many configuration fields for all kinds of uses, which were then moved to sub-types)
