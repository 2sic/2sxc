---
uid: Specs.Data.Values.Overview
---
# Value Data Types

The EAV (Entity-Attribute-Value) system and 2sxc is all about data. The data in the Attributes (aka Fields, Properties) are have a _Type_ This **Value-Type** or **Data-Type** describes how data is stored (persisted) in various formats (SQL, JSON, XML) and how it's used in code (C#, JavaScript, Tokens, Angular, ...). 

## Data Types in EAV/2sxc 10

1. [Boolean](xref:Specs.Data.Values.Boolean) - true/false values
1. [Custom](xref:Specs.Data.Values.Custom) - for internal (core development) use only, please don't use for anything else
1. [DateTime](xref:Specs.Data.Values.DateTime) - for dates and times
1. [Empty](xref:Specs.Data.Values.Empty) - for non-saved data like group-headings
1. [Entity](xref:Specs.Data.Values.Entity) - for relationships between items - like a book to the author or a blog-post to tags
1. [Hyperlink](xref:Specs.Data.Values.Hyperlink) - a special string with helper objects which resolve "file:72" to the real link
1. [Number](xref:Specs.Data.Values.Number) - for any kind of number like 1, 2, 3 or GPS coordinates
1. [String](xref:Specs.Data.Values.String) - for standard string types or when you other options don't work

## See also: Input-Types

The **Input-Type** describes how the user can enter such data - for example using a text-field or a date-picker. Learn more about the [input types](xref:Specs.Data.Inputs.All).

## History

1. Almost all types were introduced in EAV 1.0 2sxc 1.0