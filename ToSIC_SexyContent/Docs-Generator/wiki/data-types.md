# Data Types

## Purpose / Description
The EAV (Entity-Attribute-Value) system and 2sxc is all about data. Here some minimal documentation about internals.

## Difference Between Data-Type and Input-Type
The **data-type** describes how data is stored (persisted) in various formats (SQL, JSON, XML) and how it's used in code (C#, JavaScript, Tokens, Angular, ...). 

The **input-type** describes how the user can enter such data - for example using a text-field or a date-picker. Learn more about the [input types here](ui-fields).

## Basic Data Types
EAV 4.5+ and 2sxc 9.0+ currently have the following data types

1. [Boolean](data-type-boolean)
1. Custom - for internal (core development) use only, please don't use for anything else
1. [DateTime](date-type-datetime) - for dates and times
1. [Empty](data-type-empty) - for non-saved data like group-headings
1. [Entity](data-type-entity) - for relationships between items - like a book to the author or a blog-post to tags
1. [Hyperlink](data-type-hyperlink) - a special string with helper objects which resolve "file:72" to the real link
1. [Number](data-type-number) - for any kind of number like 1, 2, 3 or GPS coordinates
1. [String](data-type-string) - for standard string types or when you other options don't work


## Read also

* [input types](ui-fields) documentation about input types

## History
1. Almost all types were introduced in EAV 1.0 2sxc 1.0