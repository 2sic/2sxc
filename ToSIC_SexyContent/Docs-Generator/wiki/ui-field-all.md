# UI Field Type: All

## Purpose / Description
The **All** field type isn't really a field type in itself, it's the information every field has to describe it's label, help-text etc. 

## Features 
The basic **string** field doesn't have any features, since all the features are in the sub-types. 

## Configuring the All field-type
Any field you open will contain the UI to configure the _All_ fields. This is what it looks like:

<img src="assets/fields/all/all.png" width="100%">

* **Name** is the is the label to be shown (multi-language)  
_Important: the name is not the name used in code, which shouldn't change - this is the visible label_
* **Notes** a short help-text which will appear in a (?) bubble
* **Visible...** show this field in the UI
* **Required** if it's required
* **Default Value** what is prefilled when you create a new item
* **Disabled** if it's disabled (grayed out)
* **Validation...** what rule it must match to allow saving

## History

1. Introduced in EAV 1.0 / 2sxc 1.0