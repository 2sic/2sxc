# UI Field Type: entity

## Purpose / Description
The **entity** field is the base type for input-fields storing [entity-relationships](data-type-entity). It has sub-types.

## Features 
The basic **entity** field simply allows you to select items of a specific type, with various add/remove/create/delete features.  

## Sub-Types of Entity Fields

1. [entity-default](ui-field-entity-default) - standard selector with type, add/remove, one/multi, delete, etc.
1. [entity-query](ui-field-entity-query) for picking entities which were pre-processed in a query

## Shared Settings
All Entity-Field Types have the following settings:

<img src="/assets/fields/entity/entity.png" width="100%">

* Basic
  * **Entity Type**
* Advanced UI Settings
  * **Multiple Items**
  * **Enable Edit**
  * **Enable Create New**
  * **Enable Add Existing**
  * **Enable Remove**
  * **Enable Delete**


## History

1. Introduced in EAV 1.0 / 2sxc 1.0
2. Changed in EAV 3.0 / 2sxc 6.0 (it used to have many configuration fields for all kinds of uses, which were then moved to sub-types)
3. Enhanched in EAV 4 / 2sxc 7 when item-delete was introduced, to allow for "private" items