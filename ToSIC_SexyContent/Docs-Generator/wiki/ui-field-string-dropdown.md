# UI Field Type: string-dropdown

## Purpose / Description
Use this field type for configuring simple dropdown UI elements, storing [string/text data](data-type-string). It's an extension of the [string field type](ui-field-string).

## Features 

1. provide values to select
1. provide visible labels which are different from the stored value
2. optionally allow users to type in something different (2sxc 9.10)

## Special Behaviour

1. When the drop-down UI element finds data stored, which doesn't match any of the values it has available, it will leave that data intact unless the users selects something manually

## Configuring a String-Dropdown
This shows the configuration dialog:


<img src="assets/fields/string/string-dropdown-configuration.png" width="100%">

## Using the Dropdown with the optional free-text-entry (2sxc 9.10)
This is what the UI looks like, if _Enable Free Text_ is enabled

<img src="assets/fields/string/string-dropdown-with-free-text-initial.png" width="100%">

If the user then clicks on the I-bar icon, the input will change to text so you could type something different - like a value which wasn't suggested, or a token.

<img src="assets/fields/string/string-dropdown-with-free-text-typing.png" width="100%">


## History
1. Introduced in EAV 2.0 2sxc 2.0, originally as part of the [string field type](ui-field-string)
2. Changed in 2sxc 6.0 - Moved to it's own sub-type
3. Enhanced in 2sxc 9.10 - definition moved to json-based and added feature for enabling manual text entry
