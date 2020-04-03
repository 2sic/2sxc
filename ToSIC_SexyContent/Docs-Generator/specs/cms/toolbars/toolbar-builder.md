---
uid: Specs.Cms.Toolbars.Build
---
# Custom Toolbars - the Easy new Toolbar Building System

2sxc 10.27+ offers a new way to build and create toolbars. Instead of the difficult to use object-structure it has a much simpler commands syntax. These are specs for the command syntax. 

> [!TIP]
> Read how to use in the [How-To Customize Toolbars](xref:HowTo.Customize.Toolbars) section.

## Overview

The new toolbar builder works in this way:

1. First, take a toolbar template - typically `default` or `empty`
2. Then make changes like adding or removing buttons
3. Optionally override certain defaults
4. Render the toolbar with optional overrides

These instructions are a simple list of strings. For example:

* `toolbar=empty` tells the system to load the empty template
* `new?contentType=Person` will add a button _new_ for the type _Person_
* `edit?entityId=5593` will add an _edit_ button for the item _5593_

These instructions are provided as a JSON array of strings - like this:

```json
["toolbar=empty", "new?contentType=Person", "edit?entityId=5593"]
```

## Examples of Build-Instructions

Every build instruction consists looks as follows: 

* `""` - an empty string is valid but not very useful
* `"/this is just a comment"` - a comment if you need to document something inline
* `"new"` or `"+new"` - add a new-button
* `"-edit"` - remove a button - like when slimming down a default toolbar
* `"%delete&show=true"` - a button modifier - to change a default (the delete button is always hidden by default)
* `"group=my-group"` or `"+group=my-group"` add a button-group
* `"+group=my-group&pos=2"` add a button group at index 2 (so in the 3rd-position, 0-based)
* `"new-quote=new?contentType=Quote"` - an identifier followed by a button name - when having multiple _new_ buttons
* `"%more&color=red"` - a modifier which changes the color of the icon

## General Syntax of a Build-Instruction

The general syntax is as follows

* (build-instruction)(&build-params _optional_)(?call-params _optional_)

## Build Instructions

A build instruction consist of 1-3 parts

* an optional prefix like 
  * `+` to add a button or group (this is the default if no prefix is given)
  * `%` to modify a default thing - like change a the color of the new-button
  * `-` to remove a button or group from the list
  * `$` for system/global things (this is the default if the command is `toolbar`, `params` or `settings`)
  * `/` a comment - like `"/here comes a special add-button"`
* a command and optional addition like
  * `toolbar` - for setting the default toolbar template
    * `toolbar=default` will load the default toolbar with all standard buttons
    * `toolbar=empty` will load an empty toolbar without any buttons
  * `settings` - for configuring the toolbar settings - see below
  * `params` - for parameters which are used in all buttons - see below
  * `group` - to add a group of buttons
    * `group=my-group-name` gives the group a name, for placing buttons in it
    * `-group=
  * `add`, `edit` etc. - all known [commands of buttons](xref:Specs.Js.Commands)



## Build Parameters (`&...`)

If you have a build-instruction and need to add some parameters you must separate them with an `&` and add the parameters in url-style, like `&color=red`. 

### Build Parameters for Toolbar (`toolbar`)

The toolbar currently doesn't have any additional parameters.

### Build Parameters for Settings (`settings`)

Settings affect how the toolbar is shown. These are the known settings:

* `&color=...` - specify a different button - see [color](#colors)
* `&hover=left` or `=right` or `=none` - default is `right`
* `&class=my-class` to add one or more classes to the toolbar

### Build Parameters for Params (`params`)

Params don't have any additional settings - they only have parameters (after the `?`). See below. 

### Build Parameters for Groups (`group`)

Groups currently don't have additional parameters.

### Build Parameters for Buttons

Buttons can have the following parameters:

* `&icon=icon-class-name` allows you to set an alternate icon
* `&color=...` gives the button another - see [color](#colors)
* `&show=true` or `&show=false` - force show/hide a button  
Most common use case is to force-show or hide an existing button. For example, if you think it's ok to show the delete button, just use `delete&show=true`
* `&class=your-class` add one or more classes to the button to affect styling
* `&title=some-text` - mouseover message


## Call Parameters (`?`)

Most instructions can have additional call-parameters - separated by a `?...`. For example, this would create an additional `+` button to create a new contact:

`add-contact=add?contentType=Contact`

### Call Parameters for Toolbars & Settings

The `toolbar` and `settings` don't have any additional parameters.

### Call Parameters for Params

Params at the global level will be used by all buttons. The most common params are

* `entityId=some-number` - mainly used for edit, delete etc.
* `entityGuid=guid-guid-guid` - mainly used for delete
* `contentType=ContentTypeName` - mainly used for adding new items
* `title=some-text` - mainly used to show a title when asking to delete something
* `isPublished=true|false`
* `prefill:Field1=val1&prefill:Field2=val2` etc.
* `for=someKey` or `for=targetType,keyType,key`

for list management
* `useModuleList=true|false`
* `sortOrder=#`


### Using Prefill Parameters

Prefill parameteres are mainly used for creating new items like

`add?contentType=Book&prefill:Title=This is nice title`

The syntax is a bit special because you may need to add multiple prefill parameters, like:

`add?contentType=Book&prefill:Title=Please enter name&prefil:Author=unknown`

Since many things can go wrong with prefills, this is what you need to know

1. All prefill parameters start with `prefill:` followed by the field name. The field name is case sensitive, so you'll probably have to write `Title` instead of `title`.
1. You can use multiple prefills, like `...&prefill:Title=Hello&Intro=welcome!`
1. When using special characters which cause trouble in urls, make sure they are uriEncoded. If you do the prefill using the razor tag `@Edit.Toolbar(...)` or `@Edit.TagToolbar(...)` this happens automatically, but if you use JavaScript to prepare it, you should use [encodeURIComponent](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/encodeURIComponent) to prepare the _value_ (not the prefix `prefill:...`)

Special considerations for each type of field

1. for text fields just write the text as is like `prefill:Title=Todays News` - remember to encode if you expect special characters. When you encode things, you can also prefill html.
1. for number fields just use `prefill:Priority=47`
1. for boolean switch fields just use `prefill:UseLightbox=true`
1. for dates we suggest to use the same ISO format `prefill:PublicationDate=2020-04-01` as this will always be recognized correctly. If you use another format like `4/1/2020` you risk that the UI will auto-detect the date based on the user and maybe pick the wrong number as the month. 
1. for dates with time you should really use ISO with `Zulu` time code like `prefill:ShowFrom=2020-04-01T10:30Z`. The `Z` at the end means that it won't adjust for time zones - otherwise the UI may show a different time. 
1. for related entities (like prefilling a category) use the target Guid, like `prefill:Category=b7c1c2e1-4896-4999-a0bc-87ddf3ce31cb`. As of now, you must always use the Guid, IDs are not supported because as the app is exported and re-imported, the IDs will change. 
1. to assign multiple entities (like for category fields with multiple categories), separate them with commas, like `prefill:Category=b7c1c2e1-4896-4999-a0bc-87ddf3ce31cb,91753b4d-4932-4b22-af1c-f6ac2b76c67a`


### Using Metadata Parameters

The metadata parameters are for creating new metadata or editing existing data. There is a short and a long syntax:

`for=key` - for example, `for=file:930` - this will create metadata for a CMS-object with the ID `file:930`. 

`for=targetType,keyType,key` like `for=10,string,file:930` which has the same effect as the previous example. 

> [!NOTE]
> Metadata buttons always also need a `contentType` parameter and usually an `entityId` parameter. 
> So if an entity already has this metadata-assignment, the dialog will show an edit for that and not create a new one. 
> The entityId should be 0 when no entity exists yet. 

To use metadata, read more about metadata (TODO).



## Special Notes

### Colors

Both `settings` and `buttons` can have an `&color=...` parameter. The `...` can be just one color or two, like `&color=red,green` or `&color=brown`. 

When specifying colors you can either use standard names like `red` or HEX-codes like `FF0000`. You cannot use the `#` hash symbol, so when specifying hex-colors, always just write the characters. The button rendering system will auto-detect hex color codes. 

Note that you can also use semi-transparent hex-codes, like `FF000088`. 

### Groups

The following button groups exist by default. You can target them to add your buttons, or remove them using `-group=name`:

* `default` - the main initial buttons like edit, add etc.
* `list` - anthing having to do with the current list - move-up/down etc.
* `edit-advanced` - advanced, not-recommended buttons like `delete`
* `view` - anything related to the view - edit template etc.
* `app` - app related buttons







## Read also

* check out the list of [commands/buttons](xref:Specs.Js.Commands)


## Demo App and further links

You should find some code examples in this demo App
* TODO:


## History

1. Added to 2sxc 10.27 .01 in March 2020
1. Prefill support added in 10.27 .02 in April 2020