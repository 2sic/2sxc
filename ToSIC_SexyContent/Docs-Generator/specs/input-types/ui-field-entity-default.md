---
uid: Specs.Data.Inputs.Entity-Default
---
# Field Input-Type **entity-default**

Use this field type for configuring an entity-picker storing Entity [relationships](xref:Specs.Data.Values.Entity). It's an extension of the [entity field type](xref:Specs.Data.Inputs.Entity).

## Features 
1.  Selector where you can select entity items of a specific type
2.	Enables multiple items to select if activated
3.	Provide edit, add/remove, delete functionality if activated
4.	An order of the selected list is preserved and order can be changed with drag and drop

## Configuring an Entity-Default
No relevant settings to be configured.

## History
1.  Introduced in EAV 1.0 / 2sxc 1.0, originally as part of the [entity field type](xref:Specs.Data.Inputs.Entity)
2.	Changed in EAV 3.0 / 2sxc 6.0 (it used to have many configuration fields for all kinds of uses, which were then moved to sub-types)
3.	Enhanched in EAV 4 / 2sxc 7 when item-delete was introduced, to allow for "private" items

