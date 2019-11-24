---
uid: Specs.Data.Type.Entity
---
# Data Type: Entity (List of Entity-Items)

## Purpose / Description
**Entity** or **Item** data is a basic [data type](xref:Specs.Data.Type.Overview) and is used to mark item-relationships, like books-to-authors or blog-to-tags. 

## Storage in the SQL Database in the EAV-Model
This is stored in a special relationships-table, so internally the current DB IDs are used to track relationships. 

## Storage in the SQL Database in the JSON-Model
This is stored as an array of strings, which contain the GUIDs of the related information. 

## Specials of the Entity type: 
The entity-type has these specials it's good to know about

1. It's always a list
1. It preserves the relationship order

### Special #1: It's always a List
Since it could contain 1 or many items (and the configuration can change whenever you want), reading it always means reading a list. So you'll always use something like this (C#):

```c#
// full name of author
var fn = Book.Author[0].FullName; 

// in case you're not sure if the author was added or null, you can do
var fn2 = (Book.Author.Any() ? Book.Author[0].FirstOrDefault : "");

// This will also work in newer versions of C#
// making fullName either the name, or a null
var fn3 = Book.Author.FirstOrDefault()?.FullName;

// the following won't work!
var wontWork = Book.Author.FullName; // this won't work
```

Or the same in JavaScript:
```javascript
var fn = Book.Author[0].FullName;

// in case you're not sure if it has any
// this uses the JS-syntax which returns the last-value of an && condition  
var fn = Book.Author && Book.Author[0].FullName;
```

### Special #2: It preserves Order
If the user said a book has 2 authors:
1. Daniel
2. Abraham

Then it's sometimes usefull to preserve the order - in this case Daniel was probably the _main author_ and Abraham helped out a bit. To allow for this, the **Entity** field will keep the order of items as they were added. 

Side-effect: Sometimes you want to have an A-Z order when showing items. As the order is not auto-sorted, you will have to do this yourself if you want to have them sorted. Use [LINQ](xref:Specs.DataSources.Linq) to do that. 


## Read also

* [String fields](xref:Specs.Data.Inputs.Entity) documentation about using it in the UI

## History
1. Introduced ca. in EAV 2.0 and 2sxc 3.0